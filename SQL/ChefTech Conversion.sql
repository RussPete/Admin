
/* Manual updates after conversion

Set prices into ingredients using InitialPrices.aspx

Change "<No Vendor>" to "- No Vendor -"

Mark ingredients as No Purchase
	Ice
	Water
	Ice Cold Water

*/

CREATE FUNCTION dbo.LTrimX(@str VARCHAR(MAX)) RETURNS VARCHAR(MAX)
AS
BEGIN
IF (ASCII(LEFT(@str, 1)) < 33) BEGIN
	SET @str = STUFF(@str, 1, PATINDEX('%[^'+CHAR(0)+'-'+CHAR(32)+']%', @str) - 1, '')
END
RETURN @str
END
GO
CREATE FUNCTION dbo.RTrimX(@str VARCHAR(MAX)) RETURNS VARCHAR(MAX)
AS
BEGIN
DECLARE @trimchars VARCHAR(10)
SET @trimchars = CHAR(9)+CHAR(10)+CHAR(13)+CHAR(32)
IF (ASCII(RIGHT(@str, 1)) < 33) BEGIN
	SET @str = REVERSE(dbo.LTrimX(REVERSE(@str)))
END
RETURN @str
END
GO

-----------------------------------------------------------------------
-- Units
delete from Units

declare @NewSeed int
select @NewSeed = (IsNull(Max(UnitRno), 0)) from Units
DBCC CheckIdent('Units', Reseed, @NewSeed)

insert into Units (UnitSingle, UnitPlural, CT_UnitID, CreatedDtTm, CreatedUser) 
select UnitSing, UnitPlur, UnitID, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Units

-----------------------------------------------------------------------
-- Vendors
print 'Vendors'
delete from Vendors

--declare @NewSeed int
select @NewSeed = (IsNull(Max(VendorRno), 0)) from Vendors
DBCC CheckIdent('Vendors', Reseed, @NewSeed)

insert into Vendors (Name, CT_VendorID, CreatedDtTm, CreatedUser) 
select VendorName, VendorID, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Vendor

-----------------------------------------------------------------------
-- Ingredients
print 'Ingredients'
delete from Ingredients

--declare @NewSeed int
select @NewSeed = (IsNull(Max(IngredRno), 0)) from Ingredients
DBCC CheckIdent('Ingredients', Reseed, @NewSeed)

insert into Ingredients (Name, StockedFlg, HideFlg, CT_ItemID, CreatedDtTm, CreatedUser) 
select ItemName, Stocked, case IsActive when 0 then 1 when 1 then 0 end, ItemID, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Inv where SubRecipe <> 1

-----------------------------------------------------------------------
-- Recipes
print 'Recipes'
delete from Recipes

--declare @NewSeed int
select @NewSeed = (IsNull(Max(RecipeRno), 0)) from Recipes
DBCC CheckIdent('Recipes', Reseed, @NewSeed)

insert into Recipes (Name, SubRecipeFlg, NumServings, YieldQty, YieldUnitRno, PortionQty, PortionUnitRno, PortionPrice, BaseCostPrice, /*BaseCostPct,*/ Instructions, CT_RecipeID, CreatedDtTm, CreatedUser) 
select Replace(RecipeName, 'Copy of ', ''), SubRecipe, NumPortions, YieldQty, (select UnitRno from Units where CT_UnitID = YieldUnitID), PortionQty, 
(select UnitRno from Units where CT_UnitID = PortionUnitID), 
PortionPrice, 
(select Top 1 case when NumPortions <> 0 then RecipeCost / NumPortions else 0 end from MC_ChefTec.dbo.RecpCost where RecipeID = r.RecipeID) As BaseCostPrice,
--(select Top 1 case when NumPortions <> 0 and PortionPrice <> 0 then (RecipeCost / NumPortions) / PortionPrice * 100 else null end from MC_ChefTec.dbo.RecpCost where RecipeID = r.RecipeID) As BaseCostPct,
'<p>' +
Replace(
dbo.RTrimX(
--Replace(
Replace(
Replace(
Replace(
Replace(
Replace(
Replace(
Replace(Cast(Instructions As varchar(8000)), 
'{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Tahoma;}}
\viewkind4\uc1\pard\lang1033\f0\fs16 ', ''),
'{\rtf1\ansi\ansicpg1252\deff0{\fonttbl{\f0\fnil\fcharset0 Tahoma;}}
\viewkind4\uc1\pard\lang1033\f0\fs16 ', ''),
'{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Tahoma;}}
\viewkind4\uc1\pard\qc\lang1033\f0\fs16\par', ''),
'{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Tahoma;}}
\viewkind4\uc1\pard\lang1033\f0\fs16\par', ''),
'\par', ''),
'd
}
', ''),
'}', '')
--'- ', ''),
), 
Char(13) + Char(10), '</p><p>')	
+ '</p>',

RecipeID, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Recipe r
where RecipeName Like 'Copy of %'

-- find matching MenuItems
update mcJobMenuItems set RecipeRno = null
update mcJobMenuItems set RecipeRno = (select RecipeRno from Recipes where Name = MenuItem)
where MenuItem in (select Name from Recipes)

-- set to use default base cost pct                       if not specifically given
update Recipes set BaseCostPct = null		--where BaseCostPct = 0

-----------------------------------------------------------------------
-- VendorIngredXref
/*
delete from VendorIngredXref

--declare @NewSeed int
select @NewSeed = (IsNull(Max(VendorIngredRno), 0)) from VendorIngredXref
DBCC CheckIdent('VendorIngredXref', Reseed, @NewSeed)

insert into VendorIngredXref (VendorRno, IngredRno, PurchaseQty, PurchaseUnitRno, CreatedDtTm, CreatedUser) 
select (select VendorRno from Vendors where CT_VendorID = VendorID), (select IngredRno from Ingredients where CT_ItemID = ItemID), Quantity1, (select UnitRno from Units where CT_UnitID = UnitID1), GetDate(), 'SysConv' 
from MC_ChefTec.dbo.ConvUnit where VendorID > 0
*/

-----------------------------------------------------------------------
-- IngredConv
delete from IngredConv

--declare @NewSeed int
select @NewSeed = (IsNull(Max(IngredConvRno), 0)) from IngredConv
DBCC CheckIdent('IngredConv', Reseed, @NewSeed)

insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'pinch'),    6,   (select Top 1 UnitRno from Units where UnitSingle = 'drop'),  GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'dash'),     2,   (select Top 1 UnitRno from Units where UnitSingle = 'pinch'), GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'tsp'),     60,   (select Top 1 UnitRno from Units where UnitSingle = 'drop'),  GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'tsp'),      8,   (select Top 1 UnitRno from Units where UnitSingle = 'dash'),  GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'tbl'),     24,   (select Top 1 UnitRno from Units where UnitSingle = 'dash'),  GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'tbl'),      3,   (select Top 1 UnitRno from Units where UnitSingle = 'tsp'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'fl oz'),    2,   (select Top 1 UnitRno from Units where UnitSingle = 'tbl'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'jigger'),   3,   (select Top 1 UnitRno from Units where UnitSingle = 'tbl'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'cup'),     16,   (select Top 1 UnitRno from Units where UnitSingle = 'tbl'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'cup'),      8,   (select Top 1 UnitRno from Units where UnitSingle = 'fl oz'), GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'pt'),      16,   (select Top 1 UnitRno from Units where UnitSingle = 'fl oz'), GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'pt'),       2,   (select Top 1 UnitRno from Units where UnitSingle = 'cup'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'qt'),      32,   (select Top 1 UnitRno from Units where UnitSingle = 'fl oz'), GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'qt'),       4,   (select Top 1 UnitRno from Units where UnitSingle = 'cup'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'qt'),       2,   (select Top 1 UnitRno from Units where UnitSingle = 'pt'),    GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'qt'),   0.94635, (select Top 1 UnitRno from Units where UnitSingle = 'l'),     GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'gal'),    128,   (select Top 1 UnitRno from Units where UnitSingle = 'fl oz'), GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'gal'),     16,   (select Top 1 UnitRno from Units where UnitSingle = 'cup'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'gal'),      4,   (select Top 1 UnitRno from Units where UnitSingle = 'qt'),    GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = '#10 can'), 12,   (select Top 1 UnitRno from Units where UnitSingle = 'cup'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = '#2.5 can'), 3.5, (select Top 1 UnitRno from Units where UnitSingle = 'cup'),   GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'lb'),	   16,   (select Top 1 UnitRno from Units where UnitSingle = 'oz'),    GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'oz'), 28.3495,   (select Top 1 UnitRno from Units where UnitSingle = 'g'),     GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'lb'), 453.592,   (select Top 1 UnitRno from Units where UnitSingle = 'g'),     GetDate(), 'SysConv')
insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (0, 1, (select Top 1 UnitRno from Units where UnitSingle = 'dozen'),   12,   (select Top 1 UnitRno from Units where UnitSingle = 'ea'),    GetDate(), 'SysConv')

insert into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) 
select (select IngredRno from Ingredients where CT_ItemID = ItemID), Quantity1, (select UnitRno from Units where CT_UnitID = UnitID1), Quantity2, (select UnitRno from Units where CT_UnitID = UnitID2), GetDate(), 'SysConv' 
from MC_ChefTec.dbo.ConvUnit where ItemID in (select CT_ItemID from Ingredients)

-----------------------------------------------------------------------
-- RecipeIngredXref
Print 'RecipeIngredXref'
delete from RecipeIngredXref

--declare @NewSeed int
select @NewSeed = (IsNull(Max(RecipeIngredRno), 0)) from RecipeIngredXref
DBCC CheckIdent('RecipeIngredXref', Reseed, @NewSeed)

-- ingredients
insert into RecipeIngredXref (RecipeRno, RecipeSeq, IngredRno, UnitQty, UnitRno, CreatedDtTm, CreatedUser) 
select (select RecipeRno from Recipes where CT_RecipeID = RecipeID), RecordID, 
(select IngredRno from Ingredients where CT_ItemID = ri.ItemID), Quantity1, 
(select UnitRno from Units where CT_UnitID = UnitID1)
, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.RecpInv ri inner join MC_ChefTec.dbo.Inv i on ri.ItemID = i.ItemID
where ri.ItemID > 0
and i.SubRecipe = 0
and RecipeID in (Select CT_RecipeID From Recipes)

-- subrecipes
insert into RecipeIngredXref (RecipeRno, RecipeSeq, SubRecipeRno, UnitQty, UnitRno, CreatedDtTm, CreatedUser) 
select (select RecipeRno from Recipes where CT_RecipeID = RecipeID), RecordID, 
(select RecipeRno from Recipes where Name = i.ItemName), Quantity1, 
(select UnitRno from Units where CT_UnitID = UnitID1)
, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.RecpInv ri inner join MC_ChefTec.dbo.Inv i on ri.ItemID = i.ItemID
where ri.ItemID > 0
and i.SubRecipe = 1
and RecipeID in (Select CT_RecipeID From Recipes)

-----------------------------------------------------------------------
-- return recipes to original un-scaled quantities

-- set original (unscaled) servings and yeild
update r set 
NumServings = s.NumPortions,
YieldQty = s.YieldQty,
YieldUnitRno = (select UnitRno from Units where CT_UnitID = s.YieldUnitID)
from Recipes r inner join MC_ChefTec.dbo.RecpScale s on r.CT_RecipeID = RecipeID
where s.NumPortions <> 0

-- set original (unscaled) quantities and units for the ingredients
update x set 
UnitQty = ri.Quantity2,
UnitRno = (select UnitRno from Units where CT_UnitID = ri.UnitID2)
from RecipeIngredXref x 
inner join Recipes r on r.RecipeRno = x.RecipeRno
inner join MC_ChefTec.dbo.RecpInv ri on ri.RecipeID = r.CT_RecipeID And ri.RecordID = x.RecipeSeq 
inner join MC_ChefTec.dbo.RecpScale s on s.RecipeID = r.CT_RecipeID 
where s.NumPortions <> 0

-----------------------------------------------------------------------
-- hide unused ingredients
update Ingredients set HideFlg = 1
update Ingredients set HideFlg = 0 where IngredRno in (select IngredRno from RecipeIngredXref)

-----------------------------------------------------------------------
-- find sub recipes that are not used as such
update r set SubrecipeFlg = 0
from Recipes r 
where SubrecipeFlg = 1 
and (select Count(*) from RecipeIngredXref where SubrecipeRno = r.RecipeRno) = 0

-----------------------------------------------------------------------
-- Purchases

delete from Purchases
--declare @NewSeed int
select @NewSeed = (IsNull(Max(PurchaseRno), 0)) from Purchases
DBCC CheckIdent('Purchases', Reseed, @NewSeed)

insert into Purchases (VendorRno, PurchaseDt, PurchaseOrdNum, VendorInvNum, CT_InvoiceID, CreatedDtTm, CreatedUser) 
select 
	(select VendorRno from Vendors where CT_VendorID = i.VendorID),
	InvoiceDate, OrderNum, InvoiceNum, InvoiceID, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Invoice i where VendorID > 0

-----------------------------------------------------------------------
-- PurchaseDetails

delete from PurchaseDetails
--declare @NewSeed int
select @NewSeed = (IsNull(Max(PurchaseDetailRno), 0)) from PurchaseDetails
DBCC CheckIdent('PurchaseDetails', Reseed, @NewSeed)

insert into PurchaseDetails (PurchaseRno, IngredRno, PurchaseQty, PurchaseUnitQty, PurchaseUnitRno, Price, CreatedDtTm, CreatedUser) 
select 
	(select PurchaseRno from Purchases where CT_InvoiceID = t.InvoiceID),
	(select IngredRno from Ingredients where CT_ItemID = t.ItemID),
	1, Quantity1,
	(select UnitRno from Units where CT_UnitID = UnitID1), 
	Cost, GetDate(), 'SysConv' 
from MC_ChefTec.dbo.Trans t where InvoiceID > 0

-----------------------------------------------------------------------
-- Settings

Update Settings Set BaseCostPct = 25, AsIsBaseCostPct = 25


drop function dbo.RTrimX
drop function dbo.LTrimX


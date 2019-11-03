use MarvellousArchive

declare @ArchiveDate datetime = '2014-12-31'

----------------------------------------------------
-- Units
----------------------------------------------------

-- differences in unit descriptions
--select * from Units u inner join marvellouscate.dbo.Units mu on u.UnitRno = mu.UnitRno where u.UnitSingle <> mu.UnitSingle or u.UnitPlural <> mu.UnitPlural

-- records that will be copied over
--select * from marvellouscate.dbo.Units mu left join Units u on mu.UnitRno = u.UnitRno where u.UnitRno Is Null

-- copy over missing unit records
set Identity_insert Units on
insert into Units (UnitRno, UnitSingle, UnitPlural, HideFlg, CT_UnitID, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mu.* from marvellouscate.dbo.Units mu left join Units u on mu.UnitRno = u.UnitRno where u.UnitRno Is Null
set Identity_insert Units off

-- update records
update u
   set UnitSingle = mu.UnitSingle
      ,UnitPlural = mu.UnitPlural
	  ,HideFlg = mu.HideFlg
      ,CT_UnitID = mu.CT_UnitID
      ,CreatedDtTm = mu.CreatedDtTm
      ,CreatedUser = mu.CreatedUser
      ,UpdatedDtTm = mu.UpdatedDtTm
      ,UpdatedUser = mu.UpdatedUser
from marvellouscate.dbo.Units mu inner join Units u on mu.UnitRno = u.UnitRno where IsNull(mu.UpdatedDtTm, mu.CreatedDtTm) > IsNull(u.UpdatedDtTm, u.CreatedDtTm)


----------------------------------------------------
-- Ingredients
----------------------------------------------------

-- differences in ingredients name
--select * from Ingredients i inner join marvellouscate.dbo.Ingredients mi on i.IngredRno = mi.IngredRno where i.Name <> mi.Name

-- records that will be copied over
--select * from marvellouscate.dbo.Ingredients mi left join Ingredients i on mi.IngredRno = i.IngredRno where i.IngredRno Is Null

-- copy over missing ingredients records
set Identity_insert Ingredients on
insert into Ingredients (IngredRno, Name, DryLiquid, StockedFlg, StockedPurchaseQty, PrefBrands, RejBrands, PrefVendors, RejVendors, IntNote, CT_ItemID, NonPurchaseFlg, MenuItemAsIsFlg, HideFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mi.* from marvellouscate.dbo.Ingredients mi left join Ingredients i on mi.IngredRno = i.IngredRno where i.IngredRno Is Null
set Identity_insert Ingredients off

-- update records
update i
   set Name = mi.Name
      ,DryLiquid = mi.DryLiquid
      ,StockedFlg = mi.StockedFlg
	  ,StockedPurchaseQty = mi.StockedPurchaseQty
      ,PrefBrands = mi.PrefBrands
      ,RejBrands = mi.RejBrands
      ,PrefVendors = mi.PrefVendors
      ,RejVendors = mi.RejVendors
      ,IntNote = mi.IntNote
      ,CT_ItemID = mi.CT_ItemID
      ,NonPurchaseFlg = mi.NonPurchaseFlg
      ,MenuItemAsIsFlg = mi.MenuItemAsIsFlg
      ,HideFlg = mi.HideFlg
      ,CreatedDtTm = mi.CreatedDtTm
      ,CreatedUser = mi.CreatedUser
      ,UpdatedDtTm = mi.UpdatedDtTm
      ,UpdatedUser = mi.UpdatedUser
from marvellouscate.dbo.Ingredients mi inner join Ingredients i on mi.IngredRno = i.IngredRno where IsNull(mi.UpdatedDtTm, mi.CreatedDtTm) > IsNull(i.UpdatedDtTm, i.CreatedDtTm)


----------------------------------------------------
-- IngredConv
----------------------------------------------------

-- differences in ingredients and conversion cross-reference
--select * from IngredConv i inner join marvellouscate.dbo.IngredConv mi on i.IngredConvRno = mi.IngredConvRno where i.IngredRno <> mi.IngredRno and mi.PurchaseUnitRno = i.PurchaseUnitRno

-- records that will be copied over
--select * from marvellouscate.dbo.IngredConv mi left join IngredConv i on mi.IngredConvRno = i.IngredConvRno where i.IngredConvRno Is Null

-- copy over missing IngredConv records
set Identity_insert IngredConv on
insert into IngredConv (IngredConvRno, IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, MissingFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mi.* from marvellouscate.dbo.IngredConv mi left join IngredConv i on mi.IngredConvRno = i.IngredConvRno where i.IngredConvRno Is Null
set Identity_insert IngredConv off

update i
   set IngredRno = mi.IngredRno
      ,PurchaseQty = mi.PurchaseQty
      ,PurchaseUnitRno = mi.PurchaseUnitRno
      ,RecipeQty = mi.RecipeQty
      ,RecipeUnitRno = mi.RecipeUnitRno
      ,MissingFlg = mi.MissingFlg
      ,CreatedDtTm = mi.CreatedDtTm
      ,CreatedUser = mi.CreatedUser
      ,UpdatedDtTm = mi.UpdatedDtTm
      ,UpdatedUser = mi.UpdatedUser
from marvellouscate.dbo.IngredConv mi inner join IngredConv i on mi.IngredConvRno = i.IngredConvRno where IsNull(mi.UpdatedDtTm, mi.CreatedDtTm) > IsNull(i.UpdatedDtTm, i.CreatedDtTm)


----------------------------------------------------
-- KitchenLocations
----------------------------------------------------

-- differences in kitchen location names
--select * from KitchenLocations k inner join marvellouscate.dbo.KitchenLocations mk on k.KitchenLocRno = mk.KitchenLocRno where k.Name <> mk.Name 

-- records that will be copied over
--select * from marvellouscate.dbo.KitchenLocations mk left join KitchenLocations k on mk.KitchenLocRno = k.KitchenLocRno where k.KitchenLocRno Is Null

-- copy over missing kitchen location records
set Identity_insert KitchenLocations on
insert into KitchenLocations (KitchenLocRno, Name, SortOrder, DefaultFlg, HideFlg) 
select mk.* from marvellouscate.dbo.KitchenLocations mk left join KitchenLocations k on mk.KitchenLocRno = k.KitchenLocRno where k.KitchenLocRno Is Null
set Identity_insert KitchenLocations off

update k
set Name = mk.Name
   ,SortOrder = mk.SortOrder
   ,DefaultFlg = mk.DefaultFlg
   ,HideFlg = mk.HideFlg
from marvellouscate.dbo.KitchenLocations mk inner join KitchenLocations k on mk.KitchenLocRno = k.KitchenLocRno 


----------------------------------------------------
-- mcJobMenuCategories
----------------------------------------------------

-- differences in menu categories
--select * from mcJobMenuCategories c inner join marvellouscate.dbo.mcJobMenuCategories mc on c.Category = mc.Category

-- records that will be copied over
--select * from marvellouscate.dbo.mcJobMenuCategories mc left join mcJobMenuCategories c on mc.Category = c.Category where c.Category Is Null

-- copy over missing menu category records
insert into mcJobMenuCategories (Category, SortOrder, MultSelFlg, HideFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mc.* from marvellouscate.dbo.mcJobMenuCategories mc left join mcJobMenuCategories c on mc.Category = c.Category where c.Category Is Null

update c
set Category = mc.Category
   ,SortOrder = mc.SortOrder
   ,MultSelFlg = mc.MultSelFlg
   ,HideFlg = mc.HideFlg
   ,CreatedDtTm = mc.CreatedDtTm
   ,CreatedUser = mc.CreatedUser
   ,UpdatedDtTm = mc.UpdatedDtTm
   ,UpdatedUser = mc.UpdatedUser
from marvellouscate.dbo.mcJobMenuCategories mc inner join mcJobMenuCategories c on mc.Category = c.Category where IsNull(mc.UpdatedDtTm, mc.CreatedDtTm) > IsNull(c.UpdatedDtTm, c.CreatedDtTm)


----------------------------------------------------
-- mcJobMenuItems
----------------------------------------------------

-- differences in menu items
--select * from mcJobMenuItems i inner join marvellouscate.dbo.mcJobMenuItems mi on i.MenuItemRno = mi.MenuItemRno

-- records that will be copied over
--select * from marvellouscate.dbo.mcJobMenuItems mi left join mcJobMenuItems i on mi.MenuItemRno = i.MenuItemRno where i.MenuItemRno Is Null

-- copy over missing menu item records
set Identity_insert mcJobMenuItems on
insert into mcJobMenuItems (MenuItemRno, Category, MenuItem, ProposalMenuItem, KitchenLocRno, HotFlg, ColdFlg, HideFlg, AsIsFlg, MultSelFlg, MultItems, IngredSelFlg, CategorySortOrder, RecipeRno, ServingQuote, ServingPrice, InaccuratePriceFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mi.MenuItemRno, mi.Category, mi.MenuItem, mi.ProposalMenuItem, mi.KitchenLocRno, mi.HotFlg, mi.ColdFlg, mi.HideFlg, mi.AsIsFlg, mi.MultSelFlg, mi.MultItems, mi.IngredSelFlg, mi.CategorySortOrder, mi.RecipeRno, mi.ServingQuote, mi.ServingPrice, mi.InaccuratePriceFlg, mi.CreatedDtTm, mi.CreatedUser, mi.UpdatedDtTm, mi.UpdatedUser
from marvellouscate.dbo.mcJobMenuItems mi left join mcJobMenuItems i on mi.MenuItemRno = i.MenuItemRno where i.MenuItemRno Is Null
set Identity_insert mcJobMenuItems off

update i
   set Category = mi.Category
      ,MenuItem = mi.MenuItem
      ,ProposalMenuItem = mi.ProposalMenuItem
      ,KitchenLocRno = mi.KitchenLocRno
      ,HotFlg = mi.HotFlg
      ,ColdFlg = mi.ColdFlg
      ,HideFlg = mi.HideFlg
      ,AsIsFlg = mi.AsIsFlg
      ,MultSelFlg = mi.MultSelFlg
      ,MultItems = mi.MultItems
      ,IngredSelFlg = mi.IngredSelFlg
      ,CategorySortOrder = mi.CategorySortOrder
      ,RecipeRno = mi.RecipeRno
      ,ServingQuote = mi.ServingQuote
      ,ServingPrice = mi.ServingPrice
      ,InaccuratePriceFlg = mi.InaccuratePriceFlg
      ,CreatedDtTm = mi.CreatedDtTm
      ,CreatedUser = mi.CreatedUser
      ,UpdatedDtTm = mi.UpdatedDtTm
      ,UpdatedUser = mi.UpdatedUser
from marvellouscate.dbo.mcJobMenuItems mi inner join mcJobMenuItems i on mi.MenuItemRno = i.MenuItemRno where IsNull(mi.UpdatedDtTm, mi.CreatedDtTm) > IsNull(i.UpdatedDtTm, i.CreatedDtTm)


----------------------------------------------------
-- mcJobs
----------------------------------------------------

-- insert missing records
insert into mcJobs 
select mj.* from marvellouscate.dbo.mcJobs mj left join mcJobs j on mj.JobRno = j.JobRno where mj.JobDate <= @ArchiveDate and j.JobRno is null

-- update any records that have changed
update j
   set Customer = mj.Customer
      ,TaxExemptFlg = mj.TaxExemptFlg
      ,ContactName = mj.ContactName
      ,ContactPhone = mj.ContactPhone
      ,ContactCell = mj.ContactCell
      ,ContactFax = mj.ContactFax
      ,ContactEmail = mj.ContactEmail
      ,ResponsibleParty = mj.ResponsibleParty
      ,BookedBy = mj.BookedBy
      ,ConfirmedBy = mj.ConfirmedBy
      ,JobType = mj.JobType
      ,ProposalFlg = mj.ProposalFlg
      ,EventType = mj.EventType
      ,EventTypeDetails = mj.EventTypeDetails
      ,ServiceType = mj.ServiceType
      ,Location = mj.Location
      ,LocationDirections = mj.LocationDirections
      ,PrintDirectionsFlg = mj.PrintDirectionsFlg
      ,Vehicle = mj.Vehicle
      ,Carts = mj.Carts
      ,NumBuffets = mj.NumBuffets
      ,BuffetSpace = mj.BuffetSpace
      ,JobDate = mj.JobDate
      ,LoadTime = mj.LoadTime
      ,DepartureTime = mj.DepartureTime
      ,ArrivalTime = mj.ArrivalTime
      ,GuestArrivalTime = mj.GuestArrivalTime
      ,MealTime = mj.MealTime
      ,EndTime = mj.EndTime
      ,CeremonyTime = mj.CeremonyTime
      ,CeremonyFlg = mj.CeremonyFlg
      ,CeremonyLocation = mj.CeremonyLocation
      ,NumMenServing = mj.NumMenServing
      ,NumWomenServing = mj.NumWomenServing
      ,NumChildServing = mj.NumChildServing
      ,PricePerPerson = mj.PricePerPerson
      ,PmtMethod = mj.PmtMethod
      ,Demographics = mj.Demographics
      ,JobNotes = mj.JobNotes
      ,ProductionNotes = mj.ProductionNotes
      ,Status = mj.Status
      ,ConfirmDeadlineDate = mj.ConfirmDeadlineDate
      ,InvDate = mj.InvDate
      ,InvEventCount = mj.InvEventCount
      ,InvPricePerPerson = mj.InvPricePerPerson
      ,SubTotAmt = mj.SubTotAmt
      ,SubTotTaxPct = mj.SubTotTaxPct
      ,ServiceSubTotPctFlg = mj.ServiceSubTotPctFlg
      ,ServiceSubTotPct = mj.ServiceSubTotPct
      ,ServiceAmt = mj.ServiceAmt
      ,ServiceTaxPct = mj.ServiceTaxPct
      ,ServicePropDesc = mj.ServicePropDesc
      ,ServicePropDescCustFlg = mj.ServicePropDescCustFlg
      ,ServicePropOptions = mj.ServicePropOptions
      ,DeliverySubTotPctFlg = mj.DeliverySubTotPctFlg
      ,DeliverySubTotPct = mj.DeliverySubTotPct
      ,DeliveryAmt = mj.DeliveryAmt
      ,DeliveryTaxPct = mj.DeliveryTaxPct
      ,DeliveryPropDesc = mj.DeliveryPropDesc
      ,DeliveryPropDescCustFlg = mj.DeliveryPropDescCustFlg
      ,DeliveryPropOptions = mj.DeliveryPropOptions
      ,ChinaDesc = mj.ChinaDesc
      ,ChinaSubTotPctFlg = mj.ChinaSubTotPctFlg
      ,ChinaSubTotPct = mj.ChinaSubTotPct
      ,ChinaAmt = mj.ChinaAmt
      ,ChinaTaxPct = mj.ChinaTaxPct
      ,ChinaPropDesc = mj.ChinaPropDesc
      ,ChinaPropDescCustFlg = mj.ChinaPropDescCustFlg
      ,ChinaPropOptions = mj.ChinaPropOptions
      ,AddServiceSubTotPctFlg = mj.AddServiceSubTotPctFlg
      ,AddServiceSubTotPct = mj.AddServiceSubTotPct
      ,AddServiceAmt = mj.AddServiceAmt
      ,AddServiceTaxPct = mj.AddServiceTaxPct
      ,AddServicePropDesc = mj.AddServicePropDesc
      ,AddServicePropDescCustFlg = mj.AddServicePropDescCustFlg
      ,AddServicePropOptions = mj.AddServicePropOptions
      ,FuelTravelSubTotPctFlg = mj.FuelTravelSubTotPctFlg
      ,FuelTravelSubTotPct = mj.FuelTravelSubTotPct
      ,FuelTravelAmt = mj.FuelTravelAmt
      ,FuelTravelTaxPct = mj.FuelTravelTaxPct
      ,FuelTravelPropDesc = mj.FuelTravelPropDesc
      ,FuelTravelPropDescCustFlg = mj.FuelTravelPropDescCustFlg
      ,FuelTravelPropOptions = mj.FuelTravelPropOptions
      ,FacilitySubTotPctFlg = mj.FacilitySubTotPctFlg
      ,FacilitySubTotPct = mj.FacilitySubTotPct
      ,FacilityAmt = mj.FacilityAmt
      ,FacilityTaxPct = mj.FacilityTaxPct
      ,FacilityPropDesc = mj.FacilityPropDesc
      ,FacilityPropDescCustFlg = mj.FacilityPropDescCustFlg
      ,FacilityPropOptions = mj.FacilityPropOptions
      ,GratuitySubTotPctFlg = mj.GratuitySubTotPctFlg
      ,GratuitySubTotPct = mj.GratuitySubTotPct
      ,GratuityAmt = mj.GratuityAmt
      ,GratuityTaxPct = mj.GratuityTaxPct
      ,RentalsDesc = mj.RentalsDesc
      ,RentalsSubTotPctFlg = mj.RentalsSubTotPctFlg
      ,RentalsSubTotPct = mj.RentalsSubTotPct
      ,RentalsAmt = mj.RentalsAmt
      ,RentalsTaxPct = mj.RentalsTaxPct
      ,RentalsPropDesc = mj.RentalsPropDesc
      ,RentalsPropDescCustFlg = mj.RentalsPropDescCustFlg
      ,RentalsPropOptions = mj.RentalsPropOptions
      ,Adj1Desc = mj.Adj1Desc
      ,Adj1SubTotPctFlg = mj.Adj1SubTotPctFlg
      ,Adj1SubTotPct = mj.Adj1SubTotPct
      ,Adj1Amt = mj.Adj1Amt
      ,Adj1TaxPct = mj.Adj1TaxPct
      ,Adj1PropDesc = mj.Adj1PropDesc
      ,Adj2Desc = mj.Adj2Desc
      ,Adj2SubTotPctFlg = mj.Adj2SubTotPctFlg
      ,Adj2SubTotPct = mj.Adj2SubTotPct
      ,Adj2Amt = mj.Adj2Amt
      ,Adj2TaxPct = mj.Adj2TaxPct
      ,Adj2PropDesc = mj.Adj2PropDesc
      ,EstTotPropDesc = mj.EstTotPropDesc
      ,EstTotPropDescCustFlg = mj.EstTotPropDescCustFlg
      ,EstTotPropOptions = mj.EstTotPropOptions
      ,DepositPropDesc = mj.DepositPropDesc
      ,DepositPropDescCustFlg = mj.DepositPropDescCustFlg
      ,DepositPropOptions = mj.DepositPropOptions
      ,InvoiceMsg = mj.InvoiceMsg
      ,InvDeliveryMethod = mj.InvDeliveryMethod
      ,InvDeliveryDate = mj.InvDeliveryDate
      ,InvEmail = mj.InvEmail
      ,InvFax = mj.InvFax
      ,PmtAmt1 = mj.PmtAmt1
      ,PmtDt1 = mj.PmtDt1
      ,PmtRef1 = mj.PmtRef1
      ,PmtMethod1 = mj.PmtMethod1
      ,PmtType1 = mj.PmtType1
      ,CCReqAmt1 = mj.CCReqAmt1
      ,CCType1 = mj.CCType1
      ,CCNum1 = mj.CCNum1
      ,CCExpDt1 = mj.CCExpDt1
      ,CCSecCode1 = mj.CCSecCode1
      ,CCName1 = mj.CCName1
      ,CCAddr1 = mj.CCAddr1
      ,CCZip1 = mj.CCZip1
      ,PmtAmt2 = mj.PmtAmt2
      ,PmtDt2 = mj.PmtDt2
      ,PmtRef2 = mj.PmtRef2
      ,PmtMethod2 = mj.PmtMethod2
      ,PmtType2 = mj.PmtType2
      ,CCReqAmt2 = mj.CCReqAmt2
      ,CCType2 = mj.CCType2
      ,CCNum2 = mj.CCNum2
      ,CCExpDt2 = mj.CCExpDt2
      ,CCSecCode2 = mj.CCSecCode2
      ,CCName2 = mj.CCName2
      ,CCAddr2 = mj.CCAddr2
      ,CCZip2 = mj.CCZip2
      ,PmtAmt3 = mj.PmtAmt3
      ,PmtDt3 = mj.PmtDt3
      ,PmtRef3 = mj.PmtRef3
      ,PmtMethod3 = mj.PmtMethod3
      ,PmtType3 = mj.PmtType3
      ,CCReqAmt3 = mj.CCReqAmt3
      ,CCType3 = mj.CCType3
      ,CCNum3 = mj.CCNum3
      ,CCExpDt3 = mj.CCExpDt3
      ,CCSecCode3 = mj.CCSecCode3
      ,CCName3 = mj.CCName3
      ,CCAddr3 = mj.CCAddr3
      ,CCZip3 = mj.CCZip3
      ,CCPmtFeeFlg = mj.CCPmtFeeFlg
      ,CCFeePct = mj.CCFeePct
      ,CCFeeAmt = mj.CCFeeAmt
      ,CCFeeTaxPct = mj.CCFeeTaxPct
      ,PreTaxSubTotAmt = mj.PreTaxSubTotAmt
      ,SalesTaxTotAmt = mj.SalesTaxTotAmt
      ,InvTotAmt = mj.InvTotAmt
      ,InvBalAmt = mj.InvBalAmt
      ,MCCLinens = mj.MCCLinens
      ,DiamondLinens = mj.DiamondLinens
      ,SusanLinens = mj.SusanLinens
      ,CUELinens = mj.CUELinens
      ,Shirts = mj.Shirts
      ,Aprons = mj.Aprons
      ,DisposableFlg = mj.DisposableFlg
      ,FinalDtTm = mj.FinalDtTm
      ,FinalUser = mj.FinalUser
      ,ProcessedDtTm = mj.ProcessedDtTm
      ,ProcessedUser = mj.ProcessedUser
      ,EditProcessedDtTm = mj.EditProcessedDtTm
      ,EditProcessedUser = mj.EditProcessedUser
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
      ,InvCreatedDtTm = mj.InvCreatedDtTm
      ,InvCreatedUser = mj.InvCreatedUser
      ,InvUpdatedDtTm = mj.InvUpdatedDtTm
      ,InvUpdatedUser = mj.InvUpdatedUser
      ,CompletedDtTm = mj.CompletedDtTm
      ,CompletedUser = mj.CompletedUser
      ,ConfirmedDtTm = mj.ConfirmedDtTm
      ,ConfirmedUser = mj.ConfirmedUser
      ,PrintedDtTm = mj.PrintedDtTm
      ,PrintedUser = mj.PrintedUser
      ,InvoicedDtTm = mj.InvoicedDtTm
      ,InvoicedUser = mj.InvoicedUser
      ,CancelledDtTm = mj.CancelledDtTm
      ,CancelledUser = mj.CancelledUser
      ,InvEmailedDtTm = mj.InvEmailedDtTm
      ,InvEmailedUser = mj.InvEmailedUser
      ,PropCreatedDtTm = mj.PropCreatedDtTm
      ,PropCreatedUser = mj.PropCreatedUser
      ,PropUpdatedDtTm = mj.PropUpdatedDtTm
      ,PropUpdatedUser = mj.PropUpdatedUser
      ,PropGeneratedDtTm = mj.PropGeneratedDtTm
      ,PropGeneratedUser = mj.PropGeneratedUser
      ,PropEmailedDtTm = mj.PropEmailedDtTm
      ,PropEmailedUser = mj.PropEmailedUser
      ,ConfirmEmailedDtTm = mj.ConfirmEmailedDtTm
      ,ConfirmEmailedUser = mj.ConfirmEmailedUser
from marvellouscate.dbo.mcJobs mj inner join mcJobs j on mj.JobRno = j.JobRno where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- JobInvoicePrices
----------------------------------------------------

-- for any prices with a null rno
if exists(select * from marvellouscate.dbo.JobInvoicePrices where Rno is null)
begin
    declare @JobRno int, @Seq int
	declare JobCursor cursor for select JobRno, Seq from marvellouscate.dbo.JobInvoicePrices where Rno is null
	open JobCursor
	fetch next from JobCursor into @JobRno, @Seq
	while @@Fetch_Status = 0
	begin
		update marvellouscate.dbo.JobInvoicePrices set Rno = (Select Max(Rno) + 1 from marvellouscate.dbo.JobInvoicePrices) where JobRno = @JobRno and Seq = @Seq
		fetch next from JobCursor into @JobRno, @Seq
	end
end

-- insert missing records
insert into JobInvoicePrices
select mj.* from marvellouscate.dbo.JobInvoicePrices mj left join JobInvoicePrices j on mj.Rno = j.Rno where mj.JobRno in (select JobRno from mcJobs) and j.Rno is null 

-- update any records that have changed
update j
   set Rno = mj.Rno
      ,JobRno = mj.JobRno
      ,Seq = mj.Seq
      ,PriceType = mj.PriceType
      ,InvPerPersonCount = mj.InvPerPersonCount
      ,InvPerPersonDesc = mj.InvPerPersonDesc
      ,InvPerPersonPrice = mj.InvPerPersonPrice
      ,InvPerPersonExtPrice = mj.InvPerPersonExtPrice
      ,InvPerItemCount = mj.InvPerItemCount
      ,InvPerItemDesc = mj.InvPerItemDesc
      ,InvPerItemPrice = mj.InvPerItemPrice
      ,InvPerItemExtPrice = mj.InvPerItemExtPrice
      ,InvAllInclDesc = mj.InvAllInclDesc
      ,InvAllInclPrice = mj.InvAllInclPrice
      ,InvAllInclExtPrice = mj.InvAllInclExtPrice
      ,PropPerPersonDesc = mj.PropPerPersonDesc
      ,PropPerPersonDescCustFlg = mj.PropPerPersonDescCustFlg
      ,PropPerPersonOptions = mj.PropPerPersonOptions
      ,PropPerItemDesc = mj.PropPerItemDesc
      ,PropPerItemDescCustFlg = mj.PropPerItemDescCustFlg
      ,PropAllInclDesc = mj.PropAllInclDesc
      ,PropAllInclDescCustFlg = mj.PropAllInclDescCustFlg
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.JobInvoicePrices mj inner join JobInvoicePrices j on mj.Rno = j.Rno where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- mcJobCrew
----------------------------------------------------

-- insert missing records
insert into mcJobCrew
select mj.* from marvellouscate.dbo.mcJobCrew mj left join mcJobCrew j on mj.JobRno = j.JobRno and mj.CrewSeq = j.CrewSeq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,CrewSeq = mj.CrewSeq
      ,CrewMember = mj.CrewMember
      ,CrewAssignment = mj.CrewAssignment
      ,ReportTime = mj.ReportTime
      ,Note = mj.Note
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.mcJobCrew mj inner join mcJobCrew j on mj.JobRno = j.JobRno and mj.CrewSeq = j.CrewSeq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- mcJobDishes
----------------------------------------------------

-- insert missing records
insert into mcJobDishes
select mj.* from marvellouscate.dbo.mcJobDishes mj left join mcJobDishes j on mj.JobRno = j.JobRno and mj.DishSeq = j.DishSeq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,DishSeq = mj.DishSeq
      ,DishItem = mj.DishItem
      ,Qty = mj.Qty
      ,Note = mj.Note
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.mcJobDishes mj inner join mcJobDishes j on mj.JobRno = j.JobRno and mj.DishSeq = j.DishSeq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- mcJobFood
----------------------------------------------------

-- insert missing records
set Identity_insert mcJobFood on
insert into mcJobFood (JobFoodRno, JobRno, FoodSeq, Category, MenuItem, MenuItemRno, Qty, QtyNote, ServiceNote, ProposalSeq, ProposalMenuItem, ProposalTitleFlg, ProposalHideFlg, IngredSelFlg, IngredSel, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
select mj.JobFoodRno, mj.JobRno, mj.FoodSeq, mj.Category, mj.MenuItem, mj.MenuItemRno, mj.Qty, mj.QtyNote, mj.ServiceNote, mj.ProposalSeq, mj.ProposalMenuItem, mj.ProposalTitleFlg, mj.ProposalHideFlg, mj.IngredSelFlg, mj.IngredSel, mj.CreatedDtTm, mj.CreatedUser, mj.UpdatedDtTm, mj.UpdatedUser 
from marvellouscate.dbo.mcJobFood mj left join mcJobFood j on mj.JobRno = j.JobRno and mj.FoodSeq = j.FoodSeq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 
set Identity_insert mcJobFood off

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,FoodSeq = mj.FoodSeq
      ,Category = mj.Category
      ,MenuItem = mj.MenuItem
      ,MenuItemRno = mj.MenuItemRno
      ,Qty = mj.Qty
      ,QtyNote = mj.QtyNote
      ,ServiceNote = mj.ServiceNote
      ,ProposalSeq = mj.ProposalSeq
      ,ProposalMenuItem = mj.ProposalMenuItem
      ,ProposalTitleFlg = mj.ProposalTitleFlg
      ,ProposalHideFlg = mj.ProposalHideFlg
      ,IngredSelFlg = mj.IngredSelFlg
      ,IngredSel = mj.IngredSel
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.mcJobFood mj inner join mcJobFood j on mj.JobRno = j.JobRno and mj.FoodSeq = j.FoodSeq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- mcJobServices
----------------------------------------------------

-- insert missing records
insert into mcJobServices
select mj.* from marvellouscate.dbo.mcJobServices mj left join mcJobServices j on mj.JobRno = j.JobRno and mj.ServiceSeq = j.ServiceSeq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,ServiceSeq = mj.ServiceSeq
      ,ServiceItem = mj.ServiceItem
      ,MCCRespFlg = mj.MCCRespFlg
      ,CustomerRespFlg = mj.CustomerRespFlg
      ,Note = mj.Note
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.mcJobServices mj inner join mcJobServices j on mj.JobRno = j.JobRno and mj.ServiceSeq = j.ServiceSeq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- mcJobSupplies
----------------------------------------------------

-- insert missing records
insert into mcJobSupplies
select mj.* from marvellouscate.dbo.mcJobSupplies mj left join mcJobSupplies j on mj.JobRno = j.JobRno and mj.SupplySeq = j.SupplySeq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,SupplySeq = mj.SupplySeq
      ,SupplyItem = mj.SupplyItem
      ,Qty = mj.Qty
      ,Note = mj.Note
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.mcJobSupplies mj inner join mcJobSupplies j on mj.JobRno = j.JobRno and mj.SupplySeq = j.SupplySeq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- Payments
----------------------------------------------------

-- insert missing records
insert into Payments (JobRno,Seq, Amount, PaymentDt, Reference, Method, Type, PurchaseOrder, CCReqAmt, CCType ,CCNum, CCExpDt, CCSecCode, CCName, CCAddr, CCZip ,CCStatus, CCResult, CCAuth, CCReference, CCPmtDtTm, CCPmtUser, CCPrintDtTm, CCPrintUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
select mj.JobRno, mj.Seq, mj.Amount, mj.PaymentDt, mj.Reference, mj.Method, mj.Type, mj.PurchaseOrder, mj.CCReqAmt, mj.CCType, mj.CCNum, mj.CCExpDt, mj.CCSecCode, mj.CCName, mj.CCAddr, mj.CCZip, mj.CCStatus, mj.CCResult, mj.CCAuth, mj.CCReference, mj.CCPmtDtTm, mj.CCPmtUser, mj.CCPrintDtTm, mj.CCPrintUser, mj.CreatedDtTm, mj.CreatedUser, mj.UpdatedDtTm, mj.UpdatedUser 
from marvellouscate.dbo.Payments mj left join Payments j on mj.JobRno = j.JobRno and mj.Seq = j.Seq where mj.JobRno in (select JobRno from mcJobs union select 0) and j.JobRno is null 

-- update any records that have changed
update j
   set JobRno = mj.JobRno
      ,Seq = mj.Seq
	  ,Amount = mj.Amount
	  ,PaymentDt = mj.PaymentDt
	  ,Reference = mj.Reference
	  ,Method = mj.Method
	  ,Type = mj.Type
	  ,PurchaseOrder = mj.PurchaseOrder
	  ,CCReqAmt = mj.CCReqAmt
	  ,CCType = mj.CCType
	  ,CCNum = mj.CCNum
	  ,CCExpDt = mj.CCExpDt
	  ,CCSecCode = mj.CCSecCode
	  ,CCName = mj.CCName
	  ,CCAddr = mj.CCAddr
	  ,CCZip = mj.CCZip
	  ,CCStatus = mj.CCStatus
	  ,CCResult = mj.CCResult
	  ,CCAuth = mj.CCAuth
	  ,CCReference = mj.CCReference
	  ,CCPmtDtTm = mj.CCPmtDtTm
	  ,CCPmtUser = mj.CCPmtUser
	  ,CCPrintDtTm = mj.CCPrintDtTm
	  ,CCPrintUser = mj.CCPrintUser
      ,CreatedDtTm = mj.CreatedDtTm
      ,CreatedUser = mj.CreatedUser
      ,UpdatedDtTm = mj.UpdatedDtTm
      ,UpdatedUser = mj.UpdatedUser
from marvellouscate.dbo.Payments mj inner join Payments j on mj.JobRno = j.JobRno and mj.Seq = j.Seq where IsNull(mj.UpdatedDtTm, mj.CreatedDtTm) > IsNull(j.UpdatedDtTm, j.CreatedDtTm)


----------------------------------------------------
-- ProposalPricingOptions
----------------------------------------------------

-- differences in proposal pricing options
--select * from ProposalPricingOptions p inner join marvellouscate.dbo.ProposalPricingOptions mp on p.PropPricingRno = mp.PropPricingRno

-- records that will be copied over
--select * from marvellouscate.dbo.ProposalPricingOptions mp left join ProposalPricingOptions p on mp.PropPricingRno = p.PropPricingRno where p.PropPricingRno Is Null

-- copy over missing menu item records
set Identity_insert ProposalPricingOptions on
insert into ProposalPricingOptions (PropPricingRno, PricingCode, Seq, OptionType, OptionCheckboxDesc, OptionPropDesc, OptionDefault, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, DeletedDtTm, DeletedUser) 
select mp.PropPricingRno, mp.PricingCode, mp.Seq, mp.OptionType, mp.OptionCheckboxDesc, mp.OptionPropDesc, mp.OptionDefault, mp.CreatedDtTm, mp.CreatedUser, mp.UpdatedDtTm, mp.UpdatedUser, mp.DeletedDtTm, mp.DeletedUser
from marvellouscate.dbo.ProposalPricingOptions mp left join ProposalPricingOptions p on mp.PropPricingRno = p.PropPricingRno where p.PropPricingRno Is Null
set Identity_insert ProposalPricingOptions off

update p
   set PricingCode = mp.PricingCode
      ,Seq = mp.Seq
      ,OptionType = mp.OptionType
      ,OptionCheckboxDesc = mp.OptionCheckboxDesc
      ,OptionPropDesc = mp.OptionPropDesc
      ,OptionDefault = mp.OptionDefault
      ,CreatedDtTm = mp.CreatedDtTm
      ,CreatedUser = mp.CreatedUser
      ,UpdatedDtTm = mp.UpdatedDtTm
      ,UpdatedUser = mp.UpdatedUser
      ,DeletedDtTm = mp.DeletedDtTm
      ,DeletedUser = mp.DeletedUser
from marvellouscate.dbo.ProposalPricingOptions mp inner join ProposalPricingOptions p on mp.PropPricingRno = p.PropPricingRno where IsNull(mp.UpdatedDtTm, mp.CreatedDtTm) > IsNull(p.UpdatedDtTm, p.CreatedDtTm)


----------------------------------------------------
-- Vendors
----------------------------------------------------

-- differences in vendors
--select * from Vendors v inner join marvellouscate.dbo.Vendors mv on v.VendorRno = mv.VendorRno where v.Name <> mv.Name

-- records that will be copied over
--select * from marvellouscate.dbo.Vendors mv left join Vendors v on mv.VendorRno = v.VendorRno where v.VendorRno Is Null

-- copy over missing vendors records
set Identity_insert Vendors on
insert into Vendors (VendorRno, Name, Phone, Contact, Email, Website, Note, HideFlg, CT_VendorID, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mv.* from marvellouscate.dbo.Vendors mv left join Vendors v on mv.VendorRno = v.VendorRno where v.VendorRno Is Null
set Identity_insert Vendors off

update v
   set Name = mv.Name
      ,Phone = mv.Phone
      ,Contact = mv.Contact
      ,Email = mv.Email
      ,Website = mv.Website
      ,Note = mv.Note
      ,HideFlg = mv.HideFlg
      ,CT_VendorID = mv.CT_VendorID
      ,CreatedDtTm = mv.CreatedDtTm
      ,CreatedUser = mv.CreatedUser
      ,UpdatedDtTm = mv.UpdatedDtTm
      ,UpdatedUser = mv.UpdatedUser
from marvellouscate.dbo.Vendors mv inner join Vendors v on mv.VendorRno = v.VendorRno where IsNull(mv.UpdatedDtTm, mv.CreatedDtTm) > IsNull(v.UpdatedDtTm, v.CreatedDtTm)


----------------------------------------------------
-- Purchases
----------------------------------------------------

-- differences in purchases
--select * from Purchases p inner join marvellouscate.dbo.Purchases mp on p.PurchaseRno = mp.PurchaseRno

-- records that will be copied over
--select * from marvellouscate.dbo.Purchases mp left join Purchases p on mp.PurchaseRno = p.PurchaseRno where mp.PurchaseDt <= @ArchiveDate and p.PurchaseRno Is Null

-- copy over missing menu item records
set Identity_insert Purchases on
insert into Purchases (PurchaseRno, VendorRno, PurchaseDt, PurchaseOrdNum, VendorInvNum, Note, CT_InvoiceID, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mp.PurchaseRno, mp.VendorRno, mp.PurchaseDt, mp.PurchaseOrdNum, mp.VendorInvNum, mp.Note, mp.CT_InvoiceID, mp.CreatedDtTm, mp.CreatedUser, mp.UpdatedDtTm, mp.UpdatedUser
from marvellouscate.dbo.Purchases mp left join Purchases p on mp.PurchaseRno = p.PurchaseRno where mp.PurchaseDt <= @ArchiveDate and p.PurchaseRno Is Null
set Identity_insert Purchases off

update p
   set VendorRno = mp.VendorRno
      ,PurchaseDt = mp.PurchaseDt
      ,PurchaseOrdNum = mp.PurchaseOrdNum
      ,VendorInvNum = mp.VendorInvNum
      ,Note = mp.Note
      ,CT_InvoiceID = mp.CT_InvoiceID
      ,CreatedDtTm = mp.CreatedDtTm
      ,CreatedUser = mp.CreatedUser
      ,UpdatedDtTm = mp.UpdatedDtTm
      ,UpdatedUser = mp.UpdatedUser
from marvellouscate.dbo.Purchases mp inner join Purchases p on mp.PurchaseRno = p.PurchaseRno where IsNull(mp.UpdatedDtTm, mp.CreatedDtTm) > IsNull(p.UpdatedDtTm, p.CreatedDtTm)


----------------------------------------------------
-- PurchaseDetails
----------------------------------------------------

-- differences in purchase details
--select * from PurchaseDetails p inner join marvellouscate.dbo.PurchaseDetails mp on p.PurchaseDetailRno = mp.PurchaseDetailRno

-- records that will be copied over
--select * from marvellouscate.dbo.PurchaseDetails mp left join PurchaseDetails p on mp.PurchaseDetailRno = p.PurchaseDetailRno where p.PurchaseDetailRno Is Null

-- copy over missing menu item records
set Identity_insert PurchaseDetails on
insert into PurchaseDetails (PurchaseDetailRno, PurchaseRno, IngredRno, PurchaseQty, PurchaseUnitQty, PurchaseUnitRno, Price, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mp.PurchaseDetailRno, mp.PurchaseRno, mp.IngredRno, mp.PurchaseQty, mp.PurchaseUnitQty, mp.PurchaseUnitRno, mp.Price, mp.CreatedDtTm, mp.CreatedUser, mp.UpdatedDtTm, mp.UpdatedUser
from marvellouscate.dbo.PurchaseDetails mp left join PurchaseDetails p on mp.PurchaseDetailRno = p.PurchaseDetailRno where p.PurchaseDetailRno Is Null and mp.PurchaseRno in (select PurchaseRno from Purchases)
set Identity_insert PurchaseDetails off

update p
   set PurchaseRno = mp.PurchaseRno
      ,IngredRno = mp.IngredRno
      ,PurchaseQty = mp.PurchaseQty
      ,PurchaseUnitQty = mp.PurchaseUnitQty
      ,PurchaseUnitRno = mp.PurchaseUnitRno
      ,Price = mp.Price
      ,CreatedDtTm = mp.CreatedDtTm
      ,CreatedUser = mp.CreatedUser
      ,UpdatedDtTm = mp.UpdatedDtTm
      ,UpdatedUser = mp.UpdatedUser
from marvellouscate.dbo.PurchaseDetails mp inner join PurchaseDetails p on mp.PurchaseDetailRno = p.PurchaseDetailRno where IsNull(mp.UpdatedDtTm, mp.CreatedDtTm) > IsNull(p.UpdatedDtTm, p.CreatedDtTm)


----------------------------------------------------
-- Recipes
----------------------------------------------------

-- differences in recipes
--select * from Recipes r inner join marvellouscate.dbo.Recipes mr on r.RecipeRno = mr.RecipeRno where r.RecipeRno <> mr.RecipeRno

-- records that will be copied over
--select * from marvellouscate.dbo.Recipes mr left join Recipes r on mr.RecipeRno = r.RecipeRno where r.RecipeRno Is Null

-- copy over missing recipes records
set Identity_insert Recipes on
insert into Recipes (RecipeRno, Name, Category, SubrecipeFlg, NumServings, MenServingRatio, WomenServingRatio, ChildServingRatio, YieldQty, YieldUnitRno, PortionQty, PortionUnitRno, PortionPrice, BaseCostPrice, BaseCostPct, InaccuratePriceFlg, Instructions, Source, Images, IntNote, CT_RecipeID, MenuItemAsIsFlg, GlutenFreeFlg, VeganFlg, VegetarianFlg, DairyFreeFlg, NutsFlg, HideFlg, IncludeInBookFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mr.* from marvellouscate.dbo.Recipes mr left join Recipes r on mr.RecipeRno = r.RecipeRno where r.RecipeRno Is Null
set Identity_insert Recipes off

update r
   set Name = mr.Name
      ,Category = mr.Category
      ,SubrecipeFlg = mr.SubrecipeFlg
      ,NumServings = mr.NumServings
      ,MenServingRatio = mr.MenServingRatio
      ,WomenServingRatio = mr.WomenServingRatio
      ,ChildServingRatio = mr.ChildServingRatio
      ,YieldQty = mr.YieldQty
      ,YieldUnitRno = mr.YieldUnitRno
      ,PortionQty = mr.PortionQty
      ,PortionUnitRno = mr.PortionUnitRno
      ,PortionPrice = mr.PortionPrice
      ,BaseCostPrice = mr.BaseCostPrice
      ,BaseCostPct = mr.BaseCostPct
      ,InaccuratePriceFlg = mr.InaccuratePriceFlg
      ,Instructions = mr.Instructions
      ,Source = mr.Source
      ,Images = mr.Images
      ,IntNote = mr.IntNote
      ,CT_RecipeID = mr.CT_RecipeID
      ,MenuItemAsIsFlg = mr.MenuItemAsIsFlg
      ,GlutenFreeFlg = mr.GlutenFreeFlg
	  ,VeganFlg = mr.VeganFlg
	  ,VegetarianFlg = mr.VegetarianFlg
	  ,DairyFreeFlg = mr.DairyFreeFlg
	  ,NutsFlg = mr.NutsFlg
      ,HideFlg = mr.HideFlg
	  ,IncludeInBookFlg = mr.IncludeInBookFlg
      ,CreatedDtTm = mr.CreatedDtTm
      ,CreatedUser = mr.CreatedUser
      ,UpdatedDtTm = mr.UpdatedDtTm
      ,UpdatedUser = mr.UpdatedUser
from marvellouscate.dbo.Recipes mr inner join Recipes r on mr.RecipeRno = r.RecipeRno where IsNull(mr.UpdatedDtTm, mr.CreatedDtTm) > IsNull(r.UpdatedDtTm, r.CreatedDtTm)


----------------------------------------------------
-- RecipeIngredXref
----------------------------------------------------

-- differences in recipe ingredient cross-reference
--select * from RecipeIngredXref r inner join marvellouscate.dbo.RecipeIngredXref mr on r.RecipeIngredRno = mr.RecipeIngredRno where r.RecipeRno <> mr.RecipeRno and r.IngredRno <> mr.IngredRno

-- records that will be copied over
--select * from marvellouscate.dbo.RecipeIngredXref mr left join RecipeIngredXref r on mr.RecipeIngredRno = r.RecipeIngredRno where r.RecipeIngredRno Is Null

-- copy over missing RecipeIngredXref records
set Identity_insert RecipeIngredXref on
insert into RecipeIngredXref (RecipeIngredRno, RecipeRno, RecipeSeq, IngredRno, SubrecipeRno, UnitQty, UnitRno, BaseCostPrice, Title, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
select mr.* from marvellouscate.dbo.RecipeIngredXref mr left join RecipeIngredXref r on mr.RecipeIngredRno = r.RecipeIngredRno where r.RecipeIngredRno Is Null
set Identity_insert RecipeIngredXref off

update r
   set RecipeRno = mr.RecipeRno
      ,RecipeSeq = mr.RecipeSeq
      ,IngredRno = mr.IngredRno
      ,SubrecipeRno = mr.SubrecipeRno
      ,UnitQty = mr.UnitQty
      ,UnitRno = mr.UnitRno
      ,BaseCostPrice = mr.BaseCostPrice
      ,Title = mr.Title
      ,Note = mr.Note
      ,CreatedDtTm = mr.CreatedDtTm
      ,CreatedUser = mr.CreatedUser
      ,UpdatedDtTm = mr.UpdatedDtTm
      ,UpdatedUser = mr.UpdatedUser
from marvellouscate.dbo.RecipeIngredXref mr inner join RecipeIngredXref r on mr.RecipeIngredRno = r.RecipeIngredRno where IsNull(mr.UpdatedDtTm, mr.CreatedDtTm) > IsNull(r.UpdatedDtTm, r.CreatedDtTm)


----------------------------------------------------
-- Settings
----------------------------------------------------

delete from Settings
insert into Settings
select * from marvellouscate.dbo.settings


----------------------------------------------------
-- VendorIngredXref
----------------------------------------------------

-- differences in vendor ingredient cross-reference
--select * from VendorIngredXref v inner join marvellouscate.dbo.VendorIngredXref mv on v.VendorIngredRno = mv.VendorIngredRno where v.VendorRno <> mv.VendorRno and v.IngredRno <> mv.IngredRno

-- records that will be copied over
--select * from marvellouscate.dbo.VendorIngredXref mv left join VendorIngredXref v on mv.VendorIngredRno = v.VendorIngredRno where v.VendorIngredRno Is Null

-- copy over missing VendorIngredXref records
set Identity_insert VendorIngredXref on
insert into VendorIngredXref (VendorIngredRno, VendorRno, IngredRno, PurchaseQty, PurchaseUnitRno, PrefOrder, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) 
select mv.* from marvellouscate.dbo.VendorIngredXref mv left join VendorIngredXref v on mv.VendorIngredRno = v.VendorIngredRno where v.VendorIngredRno Is Null
set Identity_insert VendorIngredXref off

update v
   set VendorRno = mv.VendorRno
      ,IngredRno = mv.IngredRno
      ,PurchaseQty = mv.PurchaseQty
      ,PurchaseUnitRno = mv.PurchaseUnitRno
      ,PrefOrder = mv.PrefOrder
      ,CreatedDtTm = mv.CreatedDtTm
      ,CreatedUser = mv.CreatedUser
      ,UpdatedDtTm = mv.UpdatedDtTm
      ,UpdatedUser = mv.UpdatedUser
from marvellouscate.dbo.VendorIngredXref mv inner join VendorIngredXref v on mv.VendorIngredRno = v.VendorIngredRno where IsNull(mv.UpdatedDtTm, mv.CreatedDtTm) > IsNull(v.UpdatedDtTm, v.CreatedDtTm)



----------------------------------------------------
-- Cleanup Records from Database
----------------------------------------------------

use marvellouscate

delete from JobInvoicePrices where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobCrew where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobDishes where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobFood where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobServices where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobSupplies where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from Payments where JobRno in (select JobRno from mcJobs where JobDate <= @ArchiveDate)
delete from mcJobs where JobDate <= @ArchiveDate

delete from PurchaseDetails where PurchaseRno in (select PurchaseRno from Purchases where PurchaseDt <= @ArchiveDate)
delete from Purchases where PurchaseDt <= @ArchiveDate

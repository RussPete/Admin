declare @CreateDtTm  DateTime
set @CreateDtTm = '2014-11/27'

--delete from RecipeIngredXref where CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm
--delete from Ingredients where CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm--
--delete from Recipes where CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm

--insert into Ingredients (Name, MenuItemAsIsFlg, CreatedDtTm, CreatedUser) 
select Distinct MenuItem, 1, @CreateDtTm, 'SysConv' from mcJobMenuItems where AsIsFlg = 1 and RecipeRno is null and MenuItem not in (select Name from Ingredients) order by MenuItem
/*
insert into Recipes (Name, NumServings, YieldQty, YieldUnitRno, PortionQty, PortionUnitRno, MenuItemAsIsFlg, CreatedDtTm, CreatedUser) 
select Distinct MenuItem, 1, 1, 25, 1, 25, 1, @CreateDtTm, 'SysConv' from mcJobMenuItems where AsIsFlg = 1 and RecipeRno is null

insert into RecipeIngredXref (RecipeRno, RecipeSeq, IngredRno, UnitQty, UnitRno, CreatedDtTm, CreatedUser) 
select RecipeRno, 1, (select IngredRno from Ingredients where Name = r.Name and MenuItemAsIsFlg = 1), 1, 25, @CreateDtTm, 'SysConv' from Recipes r where CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm

update mcJobMenuItems set RecipeRno = (select RecipeRno from Recipes where Name = mcJobMenuItems.MenuItem and CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm),
UpdatedDtTm = @CreateDtTm, UpdatedUser = 'SysConv'
where AsIsFlg = 1 and RecipeRno is null and MenuItem in 
(select Name from Recipes where CreatedUser = 'SysConv' and CreatedDtTm = @CreateDtTm) 
*/

/*
select * from mcJobMenuItems where AsIsFlg = 1 and RecipeRno is null and MenuItem not in (select Name from Ingredients) order by MenuItem

select * from Ingredients where MenuItemAsIsFlg = 1
select * from Recipes where MenuItemAsIsFlg = 1
select * from RecipeIngredXref where RecipeRno in (select RecipeRno from Recipes where MenuItemAsIsFlg = 1)
select * from mcJobMenuItems where AsIsFlg = 1 and RecipeRno not in (Select RecipeRno from Recipes)
update mcJobMenuItems set RecipeRno = null where AsIsFlg = 1 and RecipeRno not in (Select RecipeRno from Recipes)
*/



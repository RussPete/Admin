--select * from mcJobs Where JobDate Between '1/1/2013' And '1/7/2013'
/*
select f.* from 
mcJobs j Inner Join mcJobFood f On j.JobRno = f.JobRno
Where JobDate Between '1/1/2013' And '1/7/2013'
Order By JobRno, FoodSeq
*/

select 
--case when f.Qty <> 0 then f.Qty Else j.NumMenServing + j.NumWomenServing + j.NumChildServing end as Qty, f.MenuItem, 
j.NumMenServing, NumWomenServing, NumChildServing, f.Qty, f.MenuItem, 
r.Name, 
r.NumServings, r.MenServingRatio, r.WomenServingRatio, r.ChildServingRatio,
r.YieldQty, r.YieldUnitRno, (select UnitSingle From Units where UnitRno = r.YieldUnitRno) As YieldUnit,
r.PortionQty, r.PortionUnitRno, (select UnitSingle From Units where UnitRno = r.PortionUnitRno) As PortionUnit,
x.UnitQty, x.UnitRno, (select UnitSingle From Units where UnitRno = x.UnitRno) As Unit,
i.Name, i.StockedFlg
from 
mcJobs j Inner Join mcJobFood f On j.JobRno = f.JobRno
inner join mcJobMenuItems m on f.MenuItemRno = m.MenuItemRno
inner join Recipes r on m.RecipeRno = r.RecipeRno
inner join RecipeIngredXref x on r.RecipeRno = x.RecipeRno
inner join Ingredients i on x.IngredRno = i.IngredRno
Where JobDate Between '1/1/2013' And '1/7/2013'
Order by f.MenuItem

/*
Steps:
1 Search Jobs on job dates to food to menu items to recipes, scaling job quantity of servings to recipe
2 Build list of ingredients from recipes, scaling as needed
3 Add sub recipe ingredients, as deep as necessary, scaling as needed
4 Use ingredient converions to convert and to find match to purchase details units
5 Add as is flag for menu items, when set, add system defined receipe, and ingredient for the item
*/

/*
select case when f.Qty <> 0 then f.Qty Else j.NumMenServing + j.NumWomenServing + j.NumChildServing end as Qty, f.MenuItem, 
x.UnitQty, (select UnitSingle From Units where UnitRno = x.UnitRno) As Unit,
i.Name, i.StockedFlg
from mcJobs j 
Inner Join mcJobFood f On j.JobRno = f.JobRno
inner join mcJobMenuItems m on f.MenuItemRno = m.MenuItemRno
inner join RecipeIngredXref x on m.RecipeRno = x.RecipeRno
inner join RecipeIngredXref x1 on x.SubRecipeRno = x1.RecipeRno
inner join Ingredients i on x1.IngredRno = i.IngredRno
Where JobDate Between '1/1/2013' And '1/7/2013'
Order by f.MenuItem
*/
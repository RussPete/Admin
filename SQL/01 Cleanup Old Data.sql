declare @EndDt datetime
set @EndDt = '1/1/2011'

delete from mcJobCrew Where JobRno In (select JobRno from mcJobs where createdDtTm < @EndDt)
delete from mcJobDishes Where JobRno In (select JobRno from mcJobs where createdDtTm < @EndDt)
delete from mcJobFood Where JobRno In (select JobRno from mcJobs where createdDtTm < @EndDt)
delete from mcJobServices Where JobRno In (select JobRno from mcJobs where createdDtTm < @EndDt)
delete from mcJobSupplies Where JobRno In (select JobRno from mcJobs where createdDtTm < @EndDt)

delete from mcJobs where CreatedDtTm < @EndDt

delete from mcJobMenuItems where MenuItem not in (select MenuItem from mcJobFood)
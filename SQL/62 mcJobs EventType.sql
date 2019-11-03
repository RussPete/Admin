--update mcJobs set EventTypeDetails = EventType 

update mcJobs set EventType = 'Lunch' where EventType in ('Luncheon', 'Lucheon', 'Boxed Lunches', 'business lunch')
update mcJobs set EventType = 'Wedding Anniversary' where EventType in ('50th Wedding Anniversary')
update mcJobs set EventType = 'Dinner' where EventType in ('Birthday Dinner', 'Dinner Dance', 'Fundraiser Dinner')
update mcJobs set EventType = 'Breakfast' where EventType in ('Breakfast Items')
--update mcJobs set EventType = 'Company Party' where EventType in ('Company Summer Party', 'Holiday Company Party')
update mcJobs set EventType = 'Dessert' where EventType in ('Desserts')
update mcJobs set EventType = 'Thanksgiving Meal' where EventType in ('Thanksgiving Dinner')
update mcJobs set EventType = 'Wedding Reception' where EventType in ('Wedding Open House', 'Wedding  Reception')
update mcJobs set EventType = 'Wedding Luncheon' where EventType in ('Wedding  Lunch', 'Wedding Lunch')
update mcJobs set EventType = 'Wedding Dinner' where EventType in ('Wedding Dinner/Reception')


--update mcJobs set EventType = '' where EventType in ('')
--update mcJobs set EventType = '' where EventType in ('')

select EventType, EventTypeDetails from mcJobs where CharIndex(EventType, EventTypeDetails) > 0

update mcJobs set EventType = 'Appetizers', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Appetizers', ''))) where EventTypeDetails like '%Appetizers%'
update mcJobs set EventType = 'Baby Shower', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Baby Shower', ''))) where EventTypeDetails like '%Baby Shower%'
update mcJobs set EventType = 'Birthday Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Birthday Party', ''))) where EventTypeDetails like '%Birthday Party%'
update mcJobs set EventType = 'Bridal Shower', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Bridal Shower', ''))) where EventTypeDetails like '%Bridal Shower%'
update mcJobs set EventType = 'Company Summer Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Company Summer Party', ''))) where EventTypeDetails like '%Company Summer Party%'
update mcJobs set EventType = 'Dessert', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Dessert', ''))) where EventTypeDetails like '%Dessert%'
update mcJobs set EventType = 'Drink', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Drink', ''))) where EventTypeDetails like '%Drink%'
update mcJobs set EventType = 'Family Reunion', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Family Reunion', ''))) where EventTypeDetails like '%Family Reunion%'
update mcJobs set EventType = 'Farewell', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Farewell', ''))) where EventTypeDetails like '%Farewell%'
update mcJobs set EventType = 'Funeral Luncheon', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Funeral Luncheon', ''))) where EventTypeDetails like '%Funeral Luncheon%'
update mcJobs set EventType = 'Funeral', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Funeral', ''))) where EventTypeDetails like '%Funeral%'
update mcJobs set EventType = 'Golf Tournament', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Golf Tournament', ''))) where EventTypeDetails like '%Golf Tournament%'
update mcJobs set EventType = 'High School Reunion', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'High School Reunion', ''))) where EventTypeDetails like '%High School Reunion%'
update mcJobs set EventType = 'Holiday Company Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Holiday Company Party', ''))) where EventTypeDetails like '%Holiday Company Party%'
update mcJobs set EventType = 'Company Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Company Party', ''))) where EventTypeDetails like '%Company Party%'
update mcJobs set EventType = 'Meeting', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Meeting', ''))) where EventTypeDetails like '%Meeting%'
update mcJobs set EventType = 'Open House', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Open House', ''))) where EventTypeDetails like '%Open House%'
update mcJobs set EventType = 'Retirement Open House', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Retirement Open House', ''))) where EventTypeDetails like '%Retirement Open House%'
update mcJobs set EventType = 'Retirement Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Retirement Party', ''))) where EventTypeDetails like '%Retirement Party%'
update mcJobs set EventType = 'Reunion', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Reunion', ''))) where EventTypeDetails like '%Reunion%'
update mcJobs set EventType = 'Salad', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Salad', ''))) where EventTypeDetails like '%Salad%'
update mcJobs set EventType = 'Shower', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Shower', ''))) where EventTypeDetails like '%Shower%'
update mcJobs set EventType = 'Snack', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Snack', ''))) where EventTypeDetails like '%Snack%'
update mcJobs set EventType = 'Summer Party', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Summer Party', ''))) where EventTypeDetails like '%Summer Party%'
update mcJobs set EventType = 'Tasting', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Tasting', ''))) where EventTypeDetails like '%Tasting%'
update mcJobs set EventType = 'Thanksgiving Meal', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Thanksgiving Meal', ''))) where EventTypeDetails like '%Thanksgiving Meal%'
update mcJobs set EventType = 'Walk Through', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Walk Through', ''))) where EventTypeDetails like '%Walk Through%'
update mcJobs set EventType = 'Wedding Dinner', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Wedding Dinner', ''))) where EventTypeDetails like '%Wedding Dinner%'
update mcJobs set EventType = 'Wedding Luncheon', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Wedding Luncheon', ''))) where EventTypeDetails like '%Wedding Luncheon%'
update mcJobs set EventType = 'Wedding Reception', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Wedding Reception', ''))) where EventTypeDetails like '%Wedding Reception%'
update mcJobs set EventType = 'Wedding', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Wedding', ''))) where EventTypeDetails like '%Wedding%'
update mcJobs set EventType = 'Reception', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Reception', ''))) where EventTypeDetails like '%Reception%'
update mcJobs set EventType = 'Breakfast', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Breakfast', ''))) where EventTypeDetails like '%Breakfast%'
update mcJobs set EventType = 'Lunch', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Luncheon', ''))) where EventTypeDetails like '%Luncheon%'
update mcJobs set EventType = 'Lunch', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Lunch', ''))) where EventTypeDetails like '%Lunch%'
update mcJobs set EventType = 'Dinner', EventTypeDetails = LTrim(RTrim(Replace(EventTypeDetails, 'Dinner', ''))) where EventTypeDetails like '%Dinner%'



select EventType, EventTypeDetails, Count(*) Cnt from mcJobs where EventType in (
'Appetizers',
'Baby Shower',
'Birthday Party',
'Breakfast',
'Bridal Shower',
'Company Party',
'Company Summer Party',
'Dessert',
'Dinner',
'Drink',
'Family Reunion',
'Farewell',
'Funeral',
'Funeral Luncheon',
'Golf Tournament',
'High School Reunion',
'Holiday Company Party',
'Lunch',
'Meeting',
'Open House',
'Reception',
'Retirement Open House',
'Retirement Party',
'Reunion',
'Salad',
'Shower',
'Snack',
'Summer Party',
'Tasting',
'Thanksgiving Meal',
'Walk Through',
'Wedding',
'Wedding Dinner',
'Wedding Luncheon',
'Wedding Reception'
)
group by EventType, EventTypeDetails
order by EventType, EventTypeDetails, Cnt

select EventType, EventTypeDetails, Count(*) Cnt from mcJobs where EventType not in (
'Appetizers',
'Baby Shower',
'Birthday Party',
'Breakfast',
'Bridal Shower',
'Company Party',
'Company Summer Party',
'Dessert',
'Dinner',
'Drink',
'Family Reunion',
'Farewell',
'Funeral',
'Funeral Luncheon',
'Golf Tournament',
'High School Reunion',
'Holiday Company Party',
'Lunch',
'Meeting',
'Open House',
'Reception',
'Retirement Open House',
'Retirement Party',
'Reunion',
'Salad',
'Shower',
'Snack',
'Summer Party',
'Tasting',
'Thanksgiving Meal',
'Walk Through',
'Wedding',
'Wedding Dinner',
'Wedding Luncheon',
'Wedding Reception'
)
group by EventType, EventTypeDetails
order by EventType, EventTypeDetails, Cnt

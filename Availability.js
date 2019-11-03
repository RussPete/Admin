// Availibility.js

const SUNDAY = 0;
const MONDAY = 1;
const TUESDAY = 2;
const WEDNESDAY = 3;
const THURSDAY = 4;
const FRIDAY = 5;
const SATURDAY = 6;

const FIRST_DAY = MONDAY;

const AVAILABILITY = "availability";
const LEAVE = "leave";

$(document).ready(function()
{
//	let begDate = findBegDate(new Date());
//	let endDate = new Date(begDate.getFullYear(), begDate.getMonth(), begDate.getDate() + 7);
//	$("#txtDate").val(`${begDate.getMonth() + 1}/${begDate.getDate()}/${begDate.getFullYear()}`);
	$("#txtDate").datepicker({
		showOn: "both",
		buttonImage: "Images/CalendarIcon.gif",
		buttonImageOnly: true
	});

	//$("#txtDate").change((event) =>
	//{
	//	//let begDate = findBegDate(new Date($("#txtDate").val()));
	//	//let endDate = new Date(begDate.getFullYear(), begDate.getMonth(), begDate.getDate() + 7);
	//	//clearAndLoad(sling, begDate, endDate);
	//});

//	loadData(begDate, endDate);
});

function findBegDate(dt)
{
	dt.setDate(dt.getDate() + FIRST_DAY - dt.getDay());
	dt = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate());
	return dt;
}

let sling = null;

function loadData(begDate, endDate)
{
	$("#Page").css("cursor", "wait");
	if (slingEmail && slingPassword)
	{
		sling = new Sling();
		sling.login(slingEmail, slingPassword, slingOrgId, () =>
		{
			sling.loadUsers(() =>
			{
				sling.loadGroups(null, () =>
				{
					clearAndLoad(sling, begDate, endDate);
				});
			});
		});
	}
	else
	{
		$("#Page").html("<h1>Sling user information has not been configured.</h1>");
	}
}

function clearAndLoad(sling, begDate, endDate)
{
	sling.clearCalendar();
	sling.loadCalendar(begDate, endDate, () =>
	{
		//console.log("Calendar finished");
		render(sling, begDate, endDate);
		$("#Page").css("cursor", "default");
	});
}

function render(sling, begDate, endDate)
{
	let groupNames =
	[
		"Female Service Crew",
		"Male Service Crew"
		//"Hot Kitchen",
		//"Cold Kitchen",
		//"Molly's"
	];
	let dayNames =
	[
		"Sunday",
		"Monday",
		"Tuesday",
		"Wednesday",
		"Thursday",
		"Friday",
		"Saturday"
	];

	let schedules = document.createDocumentFragment();

	for (let groupName of groupNames)
	{
		let group = sling.groupCollection.findGroupByName(groupName);
		if (group)
		{
			let header = document.createElement("h1");
			header.innerHTML = `${group.name} - Times Unavailable`;
			schedules.appendChild(header);

			// create a table for the group
			let tbl = document.createElement("table");
			schedules.appendChild(tbl);

			let tr = document.createElement("tr");
			tbl.appendChild(tr);

			// headers
			let th = document.createElement("th");
			th.innerHTML = "Employee";
			tr.appendChild(th);

			let dt = new Date(begDate);
			for (let iDay = 0; iDay < 7; iDay++)
			{
				let th = document.createElement("th");
				th.innerHTML = `${dayNames[dt.getDay()]}<br>${dt.getMonth() + 1}/${dt.getDate()}`;
				tr.appendChild(th);
				dt.setDate(dt.getDate() + 1);
			}

			// employees
			group.users.sort((a, b) =>
			{
				let rc = a.firstName.toLocaleLowerCase().localeCompare(b.firstName.toLocaleLowerCase());
				if (rc == 0)
				{
					rc = a.lastName.toLocaleLowerCase().localeCompare(b.lastName.toLocaleLowerCase());
				}

				return rc;
			});
			for (let user of group.users)
			{
				let tr = document.createElement("tr");
				tbl.appendChild(tr);

				let td = document.createElement("td");
				td.innerHTML = `${user.firstName} ${user.lastName && user.lastName.length > 0 ? user.lastName[0] : ""}`;
				tr.appendChild(td);

				// events for the employee
				let dt = new Date(begDate);
				for (let iDay = 0; iDay < 7; iDay++)
				{
					// one day at a time
					let dtEnd = new Date(dt);
					dtEnd.setDate(dtEnd.getDate() + 1);

					// look for events on the day
					let td = document.createElement("td");
					let desc = "";
					let fAllDay = false;
					for (let event of user.calendarEvents)
					{
						if (event.type == AVAILABILITY && dt.getDay() == event.begTime.getDay() ||
							event.type == LEAVE &&
							(dt <= event.begTime && event.begTime <= dtEnd ||
							 dt <= event.endTime && event.endTime <= dtEnd ||
							 event.begTime < dt && dtEnd < event.endTime))
						{
							if (event.fullDay || event.begTime < dt && dtEnd < event.endTime)
							{
								fAllDay = true;
								desc = "All Day";
							}
							else
							{
								if (!fAllDay)
								{
									desc += `${dt <= event.begTime ? timeFormat(event.begTime) : "Midnight"} - ${event.endTime <= dtEnd ? timeFormat(event.endTime) : "Midnight"}<br>`
								}
							}
						}
					}
					td.innerHTML = desc;
					tr.appendChild(td);
					dt.setDate(dt.getDate() + 1);
				}
			}
		}
	}
	$("#Page").html(schedules);
}

function timeFormat(tm)
{
	let h = tm.getHours();
	let m = tm.getMinutes();
	let ap;
	if (h < 12)
	{
		ap = "am";
	}
	else
	{
		ap = "pm";
		h -= 12;
	}
	if (h == 0)
	{
		h = 12;
	}
	m = "0" + m;
	m = m.substr(m.length - 2, 2);

	let fmt;
	if (h == 12 && m == 0)
	{
		fmt = (ap == "am" ? "Midnight" : "Noon");
	}
	else
	{
		fmt = `${h}:${m}${ap}`;
	}

	return fmt; 
}

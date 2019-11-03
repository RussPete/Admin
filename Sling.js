// Sling.js

class Sling
{
	constructor(authorization)
	{
		this._authorization = authorization || "";
    	this._userCollection = null;
    	this._groupCollection = null;
    }

	get userCollection()
	{
		return this._userCollection;
	}

	get users()
	{
		return this._userCollection.users;
	}

	get groupCollection()
	{
		return this._groupCollection;
	}

	get groups()
	{
		return this._groupCollection.groups;
	}

	loadUsers(callBack)
	{
		this._userCollection = new UserCollection(this, () =>
		{
			if (callBack)
			{
				callBack();
			};
		});
	}

	loadGroups(data, callBack)		// data is optional, used with /users/concise
	{
		this._groupCollection = new GroupCollection(this, data, () =>
		{
			if (callBack)
			{
				callBack();
			};
		});
	}

	clearCalendar()
	{
		for (let user of this.users)
		{
			user.clearCalendarEvents();
		}
	}

	loadCalendar(begDate, endDate, callBack)
	{
		this.getCalendar(begDate, endDate, (data) =>
		{
			for (let event of data)
			{
				let user = (event.user ? this._userCollection.findUser(event.user.id) : null);
				let calendarEvent = new CalendarEvent(event.id, event.type, event.fullDay, event.dtstart, event.dtend, event.summary, event.status, user);
				if (user)
				{
					user.addCalendarEvent(calendarEvent);
				}
			}
			if (callBack)
			{
				callBack();
			};
		});
	}

	login(user, password, orgId, callBack, trys)
	{
		trys = trys || 1;
		let that = this;

        $.ajax(
        {
            complete: function (response, status)
            {
                //console.log("login complete");
            },
            contentType: "application/json",
            data: JSON.stringify(
            {
                "email": user,
                "password": password,
				"orgId": orgId
            }),
            error: function (response, status, error)
            {
            	errHandler("login", response, status, error);
            	if (trys < 3)	// retry logging in up to 3 times
            	{
            		that.login(user, password, orgId, callBack, trys + 1);
            	}
            },
            headers:
			{
//				"origin": "https://admin.marvellouscatering.net"
				"origin": "http://localhost:58531"
			},
            success: (data, status, response) =>
            {
                //console.log("login success");
                //console.log(response.getResponseHeader("Authorization"));
                this._authorization = response.getResponseHeader("Authorization");

                if (callBack)
                {
                    callBack();
                }
            },
            type: "POST",
            url: "https://api.sling.is/v1/account/login"
        });
    }

	getUsers(callBack)
    {
        $.ajax(
        {
            complete: function (response, status)
            {
                //console.log("Users complete");
            },
            error: function (response, status, error)
            {
            	errHandler("getUsers", response, status, error);
            },
            headers: 
            {
                "Authorization": this._authorization
            },
            type: "GET",
            success: function (data, status, response)
            {
                //console.log("Users success");
                if (callBack)
                {
                    callBack(data);
                }
            },
            url: "https://api.sling.is/v1/users/concise"
        });
    }

	getGroups(callBack)
    {
    	$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Groups complete");
        	},
        	error: function (response, status, error)
        	{
        		errHandler("getGroups", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "GET",
        	success: function (data, status, response)
        	{
        		//console.log("Groups success");
        		if (callBack)
        		{
        			callBack(data);
        		}
        	},
        	url: "https://api.sling.is/v1/groups"
        });
    }

	getGroupUsers(id, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("GroupUsers complete");
        	},
        	error: function (response, status, error)
        	{
        		errHandler("getGroupUsers", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "GET",
        	success: function (data, status, response)
        	{
        		//console.log("GroupUsers " + data.name + " success");
        		if (callBack)
        		{
        			callBack(data);
        		}
        	},
        	url: "https://api.sling.is/v1/groups/" + id
		});
	}

	getCalendar(begDate, endDate, callBack)
	{
        $.ajax(
        {
            complete: function (response, status)
            {
                //console.log("Calendar complete");
            },
            error: function (response, status, error)
            {
            	errHandler("getCalendar", response, status, error);
            },
            headers:
            {
                "Authorization": this._authorization
            },
            type: "GET",
            success: function (data, status, response)
            {
            	//console.log("Calendar success");
                if (callBack)
                {
                    callBack(data);
                }
            },
            url: encodeURI(`https://api.sling.is/v1/calendar?dates=${begDate.toISOString()}/${endDate.toISOString()}`)
        });
    }

	getGroupId(type, name, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	error: function (response, status, error)
        	{
        		errHandler("getGroupId", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "GET",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		let id = undefined;
        		for (let srch of data)
        		{
        			if (srch.name == name)
        			{
        				id = srch.id;
        				break;
        			}
        		}
        		if (callBack)
        		{
        			callBack(id);
        		}
        	},
        	url: `https://api.sling.is/v1/groups?type=${type}`
        });
	}

	getLocationId(location, callBack)
	{
		this.getGroupId("location", location, callBack);
	}

	getPositionId(position, callBack)
	{
		this.getGroupId("position", position, callBack);
	}

	createShift(begTime, endTime, locationId, postionId, summary, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	contentType: "application/json",
        	data: JSON.stringify(
            {
            	"dtstart": begTime.toISOString(),
            	"dtend": endTime.toISOString(),
            	"location": { "id": locationId },
            	"position": { "id": postionId },
				"summary": summary
            }),
        	error: function (response, status, error)
        	{
        		errHandler("createShift", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "POST",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		if (data)
        		{
        			if (callBack)
        			{
        				callBack(data[0].id);
        			}
				}
        	},
        	url: `https://api.sling.is/v1/shifts?ignoreConflicts=true`
        });
	}

	updateShift(id, begTime, endTime, locationId, postionId, summary, callBack, errorBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	contentType: "application/json",
        	data: JSON.stringify(
            {
            	"dtstart": begTime.toISOString(),
            	"dtend": endTime.toISOString(),
            	"location": { "id": locationId },
            	"position": { "id": postionId },
            	"summary": summary
            }),
        	error: function (response, status, error)
        	{
        		errHandler("updateShift", response, status, error);
        		if (errorBack)
        		{
        			errorBack();
        		}
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "PUT",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		if (callBack)
        		{
        			callBack();
        		}
        	},
        	url: `https://api.sling.is/v1/shifts/${id}`
        });
	}

	deleteShift(id, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	error: function (response, status, error)
        	{
        		errHandler("deleteShift", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "DELETE",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		if (callBack)
        		{
        			callBack(id);
        		}
        	},
        	url: `https://api.sling.is/v1/shifts/${id}`
        });
	}

	publishShift(id, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	error: function (response, status, error)
        	{
        		errHandler("publishShift", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "POST",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		if (callBack)
        		{
        			callBack();
        		}
        	},
        	url: `https://api.sling.is/v1/shifts/sync?id=${id}`
        });
	}

	addToPrivateConversation(data, callBack)
	{
		$.ajax(
        {
        	complete: function (response, status)
        	{
        		//console.log("Calendar complete");
        	},
        	contentType: "application/json",
        	data: JSON.stringify(data),
        	error: function (response, status, error)
        	{
        		errHandler("addToPrivateConversation", response, status, error);
        	},
        	headers:
            {
            	"Authorization": this._authorization
            },
        	type: "POST",
        	success: function (data, status, response)
        	{
        		//console.log("Calendar success");
        		if (callBack)
        		{
        			callBack(data);
        		}
        	},
        	url: "https://api.sling.is/v1/conversations"
        });
	}
}

class UserCollection
{
	constructor(sling, callBack)
	{
		this._users = [];
		this._sling = sling;
		this._load(callBack);
	}

	_load(callBack)
	{
		// using /users/concise which returns groups and users
		this._sling.getUsers((data) =>
		{
			if (data.groups)
			{
				sling.loadGroups(data.groups);
			}

			let usersData = (data.users ? data.users : data);
			for (let userData of usersData)
			{
				if (userData.active)
				{
					let user = new User(userData.id, userData.name, userData.lastname, userData.email);
					this._users.push(user);
					for (let groupId of userData.groupIds)
					{
						let group = sling.groupCollection.findGroup(groupId);
						if (group)
						{
							group.users.push(user);
						}
					}
				}
			}

			if (callBack)
			{
				callBack();
			}
		});
	}

	get users()
	{
		return this._users;
	}

	findUserBy(fieldName, fieldValue)
	{
		let found = null;
		for (let srch of this._users)
		{
			if (srch[fieldName] == fieldValue)
			{
				found = srch;
				break;
			}
		}

		return found;
	}

	findUser(id)
	{
		return this.findUserBy("id", id);
	}
}

class User 
{
    constructor(id, firstName, lastName, email)
    {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this._calendarEvents = [];
    }

	addCalendarEvent(calendarEvent)
	{
		this._calendarEvents.push(calendarEvent);
	}

	get calendarEvents()
	{
		return this._calendarEvents;
	}

	clearCalendarEvents()
	{
		this._calendarEvents = [];
	}
}

class GroupCollection
{
	constructor(sling, data, callBack)
	{
		this._groups = [];
		this._sling = sling;
		this._load(data, callBack);
	}

	_load(data, callBack)
	{
		if (data)
		{
			// using /users/concise
			for (let groupId in data)
			{
				let groupData = data[groupId];
				let group = new Group(groupData.id, groupData.name, groupData.type);
				this._groups.push(group);
				// users will be connected a little later
			}
			if (callBack)
			{
				callBack();
			}
		}
		else
		{
			// using /users
			this._sling.getGroups((data) =>
			{
				for (let groupData of data)
				{
					let group = new Group(groupData.id, groupData.name, groupData.type);
					this._groups.push(group);
					this._connectUsers(group);
				}
				if (callBack)
				{
					callBack();
				}
			});
		}
	}

	_connectUsers(group)
	{
		this._sling.getGroupUsers(group.id, (data) =>
		{
			for (let groupUser of data.members)
			{
				for (let user of this._sling.users)
				{
					if (groupUser.id == user.id)
					{
						group.users.push(user);
						break;
					}
				}
			}
		});
	}

	get groups()
	{
		return this._groups;
	}

	findGroup(id)
	{
		let found = null;
		for (let srch of this._groups)
		{
			if (srch.id == id)
			{
				found = srch;
				break;
			}
		}

		return found;
	}

	findGroupByName(name)
	{
		let found = null;
		for (let srch of this._groups)
		{
			if (srch.name == name)
			{
				found = srch;
				break;
			}
		}

		return found;
	}
}

class Group
{
	constructor(id, name, type)
	{
		this.id = id;
		this.name = name;
		this.type = type;
		this.users = [];
	}
}

class CalendarEvent
{
	constructor(id, type, fullDay, begTime, endTime, summary, status, user)
	{
		this.id = id;
		this.type = type;
		this.fullDay = fullDay;
		this.begTime = new Date(Date.parse(begTime));
		this.endTime = new Date(Date.parse(endTime));
		this.summary = summary;
		this.status = status;
		this.user = user;
	}
}

function errHandler(info, response, status, error)
{
	var txt = (response.responseText.length > 0 ? JSON.parse(response.responseText) : response.responseText);
	var msg = txt.message || txt;
	console.log(`${info} error: ${error} - ${msg}`);
}

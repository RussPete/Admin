// Calendar.js
// Copyright (c) 2005-2012 PeteSoft, LLC. All Rights Reverved


//-----------------------------------------------------------------------------
function Calendar(
	dtDate, 
	Name, 
	FieldName,
	OnChange,
	HideCtrls
)
{
	this.dtDate      = new Date(dtDate);
	this.Name        = Name;
	this.FieldName   = FieldName;
	this.OnChange    = OnChange;
	this.HideCtrls   = HideCtrls;
	this.PrevClass   = "";
	this.fVisible    = false;
	this.fHide       = false;
	
	this.ButtonClicked = calButtonClicked;
	this.OnFocus       = calOnFocus;
	this.OnBlur        = calOnBlur;
	this.Show          = calShow;
	this.Hide          = calHide;
	this.SetDate       = calSetDate;
	this.MouseOver     = calMouseOver;
	this.MouseOut      = calMouseOut;
	this.PrevMonth     = calPrevMonth;
	this.NextMonth     = calNextMonth;
	this.ShowCalendar  = calShowCalendar;
	this.DateSet       = null;			// event - called when the date is set
}

//-----------------------------------------------------------------------------
function calButtonClicked()
{
	dBug("calButtonClicked fVisible " + this.fVisible);
	
	if (this.fVisible)
	{
		this.Hide();
		SetFocus(this.FieldName);
	}
	else
	{
		this.Show();
	}
}

//-----------------------------------------------------------------------------
function calOnFocus()
{
	dBug("calOnFocus");
}

//-----------------------------------------------------------------------------
function calOnBlur()
{
	dBug("calOnBlur");
	
	this.fHide = true;
	setTimeout("cal" + this.Name + ".Hide();", 200);
	
	// show the hidden controls right away or else 
	// they end up staying visibile of the OnBlur occurs
	// because another calendar receives focus and it also
	// needs to hide some of the same controls
	ShowElementList(this.HideCtrls);
}

//-----------------------------------------------------------------------------
function calShow()
{
	dBug("calShow");
	
	this.fVisible = true;
	var divStyle = GetStyle("div" + this.Name);
	var Field = GetDocObj(this.FieldName);
	divStyle.top  = GetY(Field) + Field.offsetHeight;
	divStyle.left = GetX(Field);
	SetDisplay("div" + this.Name, true);
		
	HideElementList(this.HideCtrls);

	var sJobDate = iGetStr(this.FieldName);
	this.dtDate = (sJobDate != "" ? new Date(sJobDate) : new Date());
	this.ShowCalendar();
	
	this.fHide = false;
	SetFocus(this.FieldName);
}

//-----------------------------------------------------------------------------
function calHide()
{
	dBug("calHide " + this.fHide);
	
	if (this.fHide)
	{
		this.fVisible = false;
		SetDisplay("div" + this.Name, false);

		// show the hidden controls in calOnBlur
//		ShowElementList(this.HideCtrls);
	}
}

//-----------------------------------------------------------------------------
function calSetDate(td)
{
	dBug("calSetDate");
	
	this.dtDate = new Date(td.date);
	iSetStr(this.FieldName, td.date);
	this.ShowCalendar();
	this.PrevClass = "CalSelected";
	//this.Hide();
	FocusOnNextField(this.FieldName);
	
	if (this.OnChange)
	{
		eval(this.OnChange);
	}	

/*
	if (this.DateSet)
	{
		this.DateSet();
	}
*/	
}

//-----------------------------------------------------------------------------
function calMouseOver(Element)
{
	this.PrevClass = Element.className;
	Element.className = "CalCurr";
}

//-----------------------------------------------------------------------------
function calMouseOut(Element)
{
	Element.className = this.PrevClass;
}

//-----------------------------------------------------------------------------
function calMsgOn(Element, Msg)
{
	oSetStr("divMsg", Msg);
	var divStyle = GetStyle("divMsg");
	divStyle.top  = GetY(Element) - Element.offsetHeight;
	divStyle.left = GetX(Element) + Element.offsetHeight;
	SetDisplay("divMsg", true);
}

//-----------------------------------------------------------------------------
function calMsgOff()
{
	SetDisplay("divMsg", false);
}

//-----------------------------------------------------------------------------
function calPrevMonth(Num)
{
	this.fHide = false;
	SetFocus(this.FieldName);

	for (var i = 0; i < Num; i++)
	{
		var Month = this.dtDate.getMonth() - 1;
		if (Month < 0)
		{
			this.dtDate.setFullYear(this.dtDate.getFullYear() - 1);
			Month += 12;
		}	
		this.dtDate.setMonth(Month);
	}

	this.ShowCalendar();
}

//-----------------------------------------------------------------------------
function calNextMonth(Num)
{
	this.fHide = false;
	SetFocus(this.FieldName);
	
	for (var i = 0; i < Num; i++)
	{
		var Month = this.dtDate.getMonth() + 1;
		if (Month > 11)
		{
			this.dtDate.setFullYear(this.dtDate.getFullYear() + 1);
			Month -= 12;
		}
		this.dtDate.setMonth(Month);
	}	
	
	this.ShowCalendar();
}

//-----------------------------------------------------------------------------
function calShowCalendar()
{
	var FirstDayOfMonth = new Date(this.dtDate);
	FirstDayOfMonth.setDate(1);
	var FirstDay = new Date(FirstDayOfMonth);
	var OneDay = 24 * 60 * 60 * 1000;
	FirstDay.setTime(FirstDay.getTime() -1 * FirstDayOfMonth.getDay() * OneDay);
	var Dt = new Date(FirstDay);

	var Months = Array(
		"January", "February", "March", "April", "May", "June", 
		"July", "August", "September", "October", "November", "December");
	var Field = "td" + this.Name + "MonthYear";
	var MonthYear = Months[FirstDayOfMonth.getMonth()] + " " + FirstDayOfMonth.getFullYear();
	oSetStr(Field, MonthYear);
	
	var tbl = GetDocObj("tbl" + this.Name);
	if (tbl)
	{
		var Today = new Date();
		Today.setHours(0, 0, 0, 0);
		
		var iWeek;
		var iDay;
		var sJobDate = iGetStr(this.FieldName);
		var dtJobDate = (sJobDate != "" ? new Date(sJobDate) : new Date());
		
		for (iWeek = 0; iWeek < 6; iWeek++)
		{
			for (iDay = 0; iDay < 7; iDay++)
			{
				var Class = "";
				if (Dt.getMonth() != this.dtDate.getMonth())	{ Class = "CalOtherMonth"; }
				if (Dt.valueOf() == dtJobDate.valueOf())		{ Class = "CalSelected"; }
				
				var c = tbl.rows[2 + iWeek].cells[iDay];
				c.innerText = Dt.getDate();
				c.style.fontWeight = (Dt.valueOf() == Today.valueOf() ? "bold" : "normal");
				c.className = Class;
				var PrevDay = Dt.getDate();
				c.date = (Dt.getMonth() + 1) + "/" + Dt.getDate() + "/" + Dt.getFullYear();
				Dt.setTime(Dt.getTime() + OneDay);
				// fix for change to daylight savings time
				if (Dt.getDate() == PrevDay)
				{
					Dt.setTime(Dt.getTime() + OneDay);
				}
				Dt.setHours(0, 0, 0, 0);
			}
		}
	}
	
	this.dtDate = FirstDayOfMonth;
}

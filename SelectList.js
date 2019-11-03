// SelectList.js
// Copyright (c) 2005-2012 PeteSoft, LLC. All Rights Reverved

//-----------------------------------------------------------------------------
function SelectList
(
	Name, 
	FieldName, 
	NextField,
	OnChange,
	fAutoPostBack, 
	fAutoWidth, 
	StyleClass, 
	SelectStyleClass
)
{
	dBug("SelectList " + Name);

	this.Name             = Name;
	this.FieldName        = FieldName;
	this.NextField        = NextField;
	this.OnChange         = OnChange;
	this.fAutoPostBack    = fAutoPostBack;
	this.fAutoWidth       = fAutoWidth;
	this.StyleClass       = StyleClass;
	this.SelectStyleClass = SelectStyleClass;
	this.iCurr            = 0;
	this.fFilter          = false;
	this.fVisible         = false;
	this.fHide            = false;
	
	this.ButtonClicked      = slButtonClicked;
	this.OnFocus            = slOnFocus;
	this.OnBlur             = slOnBlur;
	this.Show               = slShow;
	this.Hide               = slHide;
	this.ValueKeyDown       = slValueKeyDown;
	this.SlideDown          = slSlideDown;
	this.SlideUp            = slSlideUp;
	this.FindFirstMatch     = slFindFirstMatch;
	this.ValueKeyUp         = slValueKeyUp;
	this.Select             = slSelect;
	this.DivFocus           = slDivFocus;		
}

//-----------------------------------------------------------------------------
function slButtonClicked()
{
	dBug("ButtonClicked " + this.Name + " fVisible " + this.fVisible + " fFilter " + this.fFilter);
	
	var fShowValues = true;

	if (this.fVisible)
	{
		if (!this.fFilter)
		{
			this.fHide = true;
			//this.Hide();
			SetFocus(this.FieldName);
			fShowValues = false;
		}
	}
	
	if (fShowValues)
	{
		this.fFilter = false;
		this.Show();
		this.FindFirstMatch();
	}
}

//-----------------------------------------------------------------------------
function slOnFocus()
{
	dBug("slOnFocus " + this.Name);

	var Value = Trim(iGetStr(this.FieldName));
	if (Value == "")
	{
		this.fFilter = false;
		this.Show();
	}
}

//-----------------------------------------------------------------------------
function slOnBlur()
{
	dBug("slOnBlur " + this.Name);
	
	this.fHide = true;
	setTimeout("sl" + this.Name + ".Hide();", 200);
}

//-----------------------------------------------------------------------------
function slShow()
{
	dBug("ShowValues " + this.Name);

	// close all other select lists
	slCloseAll(this.Name);

	var Value = iGetStr(this.FieldName);
	var tbl = GetDocObj("tbl" + this.Name + "Values");
	var cValues = 0;
	var i;

	// the first row is a spacer image and always needs to be shown
	tbl.rows[0].className = this.StyleClass;
	
	for (i = 1; i < tbl.rows.length; i++)
	{
		var c = tbl.rows[i].cells[0];
		
		var fMatch = true;		
		if (this.fFilter)
		{
			fMatch = (Value.toUpperCase() == c.innerText.substr(0, Value.length).toUpperCase());
		}
		
		tbl.rows[i].style.display = (fMatch ? "" : "none");
		tbl.rows[i].className = this.StyleClass;
		if (fMatch) { cValues++; }
	}

	if (this.iCurr > 0 && this.iCurr < tbl.rows.length)
	{
		tbl.rows[this.iCurr].className = this.SelectStyleClass;
	}

//	if (cValues > 0)
	{
		this.fVisible = true;
		SetDisplay("div" + this.Name, true);
		var divStyle = GetStyle("div" + this.Name);
		var Field = GetDocObj(this.FieldName);
		divStyle.top  = GetY(Field) + Field.offsetHeight;
		divStyle.left = GetX(Field);
		if (this.fAutoWidth)
		{
			// set a min width
			var ImgStyle = GetStyle("img" + this.Name + "Width");
			if (ImgStyle)
			{
				ImgStyle.width = Field.offsetWidth - 8;
			}
		}
		else
		{
			// set a fixed width
			divStyle.width = Field.offsetWidth;
		}
	}	
	
	this.fHide = false;
	SetFocus(this.FieldName);
}

//-----------------------------------------------------------------------------
function slHide()
{
	dBug("HideValues " + this.Name + " fHide " + this.fHide);

	if (this.fHide)
	{
		this.fVisible = false;
		SetDisplay("div" + this.Name, false);
	}
}


//-----------------------------------------------------------------------------
function slValueKeyDown(event)
{
	var rc = true;
	
	switch (event.keyCode)
	{
		case 9:		// tab
			this.Hide();
			break;

//try to see if hitting a tab works ok for selecting the current value and moving on to the next field.
//			var tbl = GetDocObj("tbl" + this.Name + "Values");
//			if (tbl && this.fVisible)
//			{
//				if (this.iCurr > 0 && this.iCurr < tbl.rows.length)
//				{
//					this.Hide();
//					var c = tbl.rows[this.iCurr].cells[0];
//					slSelectValue(this.FieldName, c.innerText, this.OnChange, this.fAutoPostBack, this.NextField);
//					rc = false;
//				}
//				else
//				{
//					this.Hide();
//				}
//			}
//			break;
			
		case 13:	// enter
			var tbl = GetDocObj("tbl" + this.Name + "Values");
			if (tbl)
			{
				if (this.iCurr >= 0 && this.iCurr < tbl.rows.length)
				{
					this.Hide();
					var c = tbl.rows[this.iCurr].cells[0];
					slSelectValue(this.FieldName, c.innerText, this.OnChange, this.fAutoPostBack, this.NextField);
					rc = false;
				}
				else
				{
					rc = false;
				}
			}
			break;
		
		case 40:	// down-arrow
			if (this.fVisible)
			{
//				if (!this.SlideDown())
				{
//					this.iCurr = 0;
					this.SlideDown();
				}
			}
			else
			{
				this.Show();
				this.FindFirstMatch();
			}
			break;
			
		case 38:	// up-arrow
//			if (!this.SlideUp())
			{
//				this.iCurr = 0;
				this.SlideUp();
			}
			break;
			
		default:
//			alert("key " + event.keyCode);
			break;

	}
	
	return rc;
}

//-----------------------------------------------------------------------------
function slSlideDown()
{
	var fFound = true;
	var tbl = GetDocObj("tbl" + this.Name + "Values");
	var iNew = this.iCurr;
	var iPrev = this.iCurr;
	
	if (this.iCurr < tbl.rows.length - 1)
	{
		fFound = false;
		while (iNew < tbl.rows.length - 1)
		{
			if (tbl.rows[++iNew].style.display == "")
			{
				tbl.rows[iNew].className = this.SelectStyleClass;

				if (iPrev > 0 && iPrev < tbl.rows.length)
				{
					tbl.rows[iPrev].className = this.StyleClass;
				}
				this.iCurr = iNew;
				fFound = true;
				break;
			}
		}
	}
	
	return fFound;
}

//-----------------------------------------------------------------------------
function slSlideUp()
{
	var fFound = true;
	var tbl = GetDocObj("tbl" + this.Name + "Values");
	
//	if (this.iCurr == 1)
//	{
//		this.iCurr = tbl.rows.length;
//	}
	
	var iNew = this.iCurr;
	var iPrev = this.iCurr;

	if (this.iCurr > 0)
	{
		fFound = false;
		while (iNew > 1)
		{
			if (tbl.rows[--iNew].style.display == "")
			{
				tbl.rows[iNew].className = this.SelectStyleClass;

				if (iPrev > 0 && iPrev < tbl.rows.length)
				{
					tbl.rows[iPrev].className = this.StyleClass;
				}
				this.iCurr = iNew;
				fFound = true;
				break;
			}
		}
	}
	
	return fFound;
}

//-----------------------------------------------------------------------------
function slFindFirstMatch()
{
	var Value = iGetStr(this.FieldName);
	var tbl = GetDocObj("tbl" + this.Name + "Values");
	var cValues = 0;
	var i;

	for (i = 1; i < tbl.rows.length; i++)
	{
		var c = tbl.rows[i].cells[0];
		
		if (Value.toUpperCase() == c.innerText.substr(0, Value.length).toUpperCase())
		{
			// hide the current selection
			if (this.iCurr > 0 && this.iCurr < tbl.rows.length)
			{
				tbl.rows[this.iCurr].className = this.StyleClass;
			}
			
			// show the one found
			this.iCurr = i;
			tbl.rows[i].className = this.SelectStyleClass;
			break;
		}
	}
}

//-----------------------------------------------------------------------------
function slValueKeyUp()
{	
//	alert(event.keyCode);
/*
	8 - backspace
	32 - space
	46 - del
	48 - 0
	57 - 9
	65 - a
	90 - z
	96 - 0 numpad
	105 - 9 numpad
	106 - * numpad
	107 - + numpad
	109 - - numpad
	110 - . numpad
	111 - / numpad
	122 - '
	186 - ;
	187 - =
	188 - ,
	189 - -
	190 - .
	191 - /
	192 - `
	219 - [
	221 - ]
*/
	switch (event.keyCode)
	{
		case 8:
		case 32:
		case 46:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 78:
		case 79:
		case 80:
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
		case 87:
		case 88:
		case 89:
		case 90:
		case 96:
		case 97:
		case 98:
		case 99:
		case 100:
		case 101:
		case 102:
		case 103:
		case 104:
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 111:
		case 122:
		case 186:
		case 187:
		case 188:
		case 189:
		case 190:
		case 191:
		case 192:
		case 219:
		case 221:
			this.fFilter = true;
			this.Show();
			break;
	}
/*	
	if (event.keyCode == 8 ||
		(event.keyCode >= 32 && event.keyCode <= 128))
	{ 
		this.fFilter = true;
		this.Show();
	}
*/
}

//-----------------------------------------------------------------------------
function slSelect(Cell)
{
	slSelectValue(this.FieldName, Cell.innerText, this.OnChange, this.fAutoPostBack, this.NextField);
}

//-----------------------------------------------------------------------------
function slDivFocus()
{
	SetFocus(this.FieldName);
}

//-----------------------------------------------------------------------------
function sl__doPostBack(eventTarget, eventArgument) {
	var theform;
	if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1) {
		theform = document.form;
	}
	else {
		theform = document.forms["form"];
	}
	theform.__EVENTTARGET.value = eventTarget.split("$").join(":");
	theform.__EVENTARGUMENT.value = eventArgument;
	theform.submit();
}

//-----------------------------------------------------------------------------
function slSelectValue(FieldName, Text, OnChange, fAutoPostBack, NextField)
{
	iSetStr(FieldName, Text);

	if (OnChange)
	{
		eval(OnChange);
	}	
	
	if (fAutoPostBack)
	{
		try
		{
			__doPostBack(FieldName, '');
		}
		catch (err)
		{
			sl__doPostBack(FieldName, '');
		}
	}
	else
	{
		if (NextField)
		{
			SetFocus(NextField);
		}
		else
		{
			FocusOnNextField(FieldName);
		}
	}
}


// Calc.js
// Copyright (c) 2005-2012 PeteSoft, LLC. All Rights Reverved

var fIE;
var fDOM;


var dBugWin = null;

//-------------------------------------------------------------------------
function dBug(Str)
{
	if (0 == 1)
	{
		if (dBugWin == null)
		{
			dBugWin = window.open("blank");
		}
		
		dBugWin.document.writeln(Str + "<br>");
	}
}

//-------------------------------------------------------------------------
// initialize values
SetupBrowser();

//-------------------------------------------------------------------------
function iGetNum(FldName)
{
	var Value = "";
	var e = GetDocObj(FldName);
	if (e)
	{
		Value = e.value;
	}
	
	return Num(Value);
}

//-------------------------------------------------------------------------
function iGetStr(FldName)
{
	var Value = "";
	var e = GetDocObj(FldName);
	if (e)
	{
		Value = e.value;
	}
	
	return Value;
}

//-------------------------------------------------------------------------
function iSetStr(FldName, Value)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.value = Value;
	}
}

//-------------------------------------------------------------------------
function oGetStr(FldName)
{
	var Value = "";
	var e = GetDocObj(FldName);
	if (e)
	{
		Value = e.innerHTML;
	}
	
	return Value;
}

//-------------------------------------------------------------------------
function oSetStr(FldName, Value)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.innerHTML = Value;
	}
}

//-------------------------------------------------------------------------
function iGetChk(FldName)
{
	var Checked = false;
	var e = GetDocObj(FldName);
	if (e)
	{
		Checked = e.checked;
	}
	
	return Checked;
}

//-------------------------------------------------------------------------
function iSetChk(FldName, fChecked)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.checked = fChecked
	}
}

//-------------------------------------------------------------------------
function iGetSel(FldName)
{
	var Value = false;
	var e = GetDocObj(FldName);
	if (e)
	{
		Value = e.options[e.selectedIndex].value;
	}
	
	return Value;
}

//-------------------------------------------------------------------------
function iGetSelStr(FldName)
{
	var Value = false;
	var e = GetDocObj(FldName);
	if (e)
	{
		Value = e.options[e.selectedIndex].text;
	}
	
	return Value;
}

//-------------------------------------------------------------------------
function iSetSel(FldName, Value)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.value = Value;
	}
}

//-------------------------------------------------------------------------
function iGetDis(FldName)
{
	var fDisabled = false;
	var e = GetDocObj(FldName);
	if (e)
	{
		fDisabled = e.disabled;
	}
	
	return fDisabled;
}

//-------------------------------------------------------------------------
function iSetDis(FldName, fDisabled)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.disabled = fDisabled;
	}
}

//-------------------------------------------------------------------------
function SetFocus(FldName)
{
	var e = GetDocObj(FldName);
	if (e)
	{
		e.focus();
	}
}

//-------------------------------------------------------------------------
function GetDocObj(FldName) 
{
	var e = null;
	
	if (document.getElementById)
	{
		e = document.getElementById(FldName);
	}
	else
	if (document.all)
	{
		e = document.all[FldName];
	}
	else
	if (document.layers) 
	{
		e = document.layers[FldName];
	}

	if (e == null)
	{
		e = eval("document.forms[0]." + FldName);
	}
	
	return e;
}

//-------------------------------------------------------------------------
function SetImg(Element, Image)
{
	Element.src = Image;
}

//-------------------------------------------------------------------------
// ZeroMode (optional), uses value of ZeroMode when Value is zero
//  "-" use '-' when zero
//  ""  use blank when zero
function FmtDollar(Value, ZeroMode)
{
	var Str = "";
	
	if (arguments.length == 1)
	{
		Str = "$" + FmtDec(Value);
	}
	else
	{
		Str = FmtDec(Value, ZeroMode);
		if (Str != ZeroMode)
		{
			Str = "$" + Str;
		}
	}

	return Str;
}

//-------------------------------------------------------------------------
// ZeroMode (optional), uses value of ZeroMode when Value is zero
//  "-" use '-' when zero
//  ""  use blank when zero
function FmtPct(Value, DecDigits, ZeroMode)
{
	if (arguments.length < 3)
	{
		ZeroMode = "";
	}
	
	var Pct = FmtNum(Value, DecDigits, ZeroMode);
	if (Pct != "")
	{
		Pct += "%";
	}
	
	return Pct;
}

//-------------------------------------------------------------------------
// ZeroMode (optional), uses value of ZeroMode when Value is zero
//  "-" use '-' when zero
//  ""  use blank when zero
function FmtDec(Value, ZeroMode)
{
	var Val = NumRnd(Value, 2);
	var Str = "";
	
	if (isNaN(Val))
	{
		Val = 0;
	}

	Str = FmtNum(Val, 2);
		
	if (Str == "0.00" && arguments.length >= 2)
	{
		Str = ZeroMode;
	}

	return Str;
}

//-------------------------------------------------------------------------
// formats the number by adding commas. the optional parm DecDigits limits 
// the number of digits after the decimal
// ZeroMode (optional), uses value of ZeroMode when Value is zero
//  "-" use '-' when zero
//  ""  use blank when zero
function FmtNum(Value, DecDigits, ZeroMode)
{
	if (isNaN(Value))
	{
		Value = 0;
	}
	
	var Num = Value;
	var fNeg = false;
	var i;

	// keep track of the numbers sign
	if (Num < 0)
	{
		fNeg = true;
		Num = -Num;
	}
	
	// check for the optional DecDigits parm
	if (arguments.length < 2) // if DecDigits not a parm
	{
		// look for a decimal point to default DecDigits
		var str = "" + Num;
		i = str.indexOf(".");
		if (i >= 0)
		{
			DecDigits = str.length - i - 1;
		}
		else
		{
			// not decimal point and digits
			DecDigits = -1;
		}
	}
	
	if (DecDigits >= 0)
	{
		// round the value
		Num = NumRnd(Num, DecDigits);
	
		// convert to a string
		Num = "" + Num;
		// if not decimal point yet, add it
		if (Num.indexOf(".") < 0)
		{
			Num += ".";
		}
		
		// add some extra zeros to fill in the decimal digits
		for (i = 0; i < DecDigits; i++)
		{
			Num += "0";
		}
	}
	else
	{	
		// convert to a string
		Num = "" + Num;
	}

	var Str = "";

	// format the decimal point and the digits
	i = Num.indexOf(".");
	if (i >= 0)
	{
		Str = (DecDigits > 0 ? Num.substr(i, DecDigits + 1) : "");
	}
	else
	{
		i = Num.length;
	}

	// format digits left of the decimal
	var Digits = 0;	
	for (i--; i >= 0; i--)
	{
		Str = Num.charAt(i) + Str;

		if (++Digits % 3 == 0 && i > 0)
		{
			Str = "," + Str;
		}
	}

	// remember the the sign of the number
	if (fNeg)
	{
		Str = "-" + Str;
	}

	// check for zero value and Mode
	if (parseFloat(Str) == 0 && arguments.length >= 3)
	{
		Str = ZeroMode;
	}
			
	return Str;
}

//-------------------------------------------------------------------------
// format the field flushed to the right with spaces
function Fmt(Value, Size)
{
	var Str = Repeat(" ", Size) + Value
	Str = Str.substr(Str.length - Size);
	
	return Str;
}

//-------------------------------------------------------------------------
// repeat the Chr Cnt times
function Repeat(Chr, Cnt)
{
	var Str = "";
	while (--Cnt > 0)
	{
		Str += Chr;
	}
	
	return Str;
}

//-------------------------------------------------------------------------
function Trim(Str)
{
	var Trimmed = "";
	var iBeg = 0;
	var iEnd = Str.length - 1;
	
	while (iBeg < iEnd && Str.charAt(iBeg) == ' ')
	{
		iBeg++;
	}

	while (iBeg < iEnd && Str.charAt(iEnd) == ' ')
	{
		iEnd++;
	}
	
	Trimmed = Str.substring(iBeg, iEnd + 1);
	
	return Trimmed
}

//-------------------------------------------------------------------------
function NumRnd(Value, DecDigits)
{
	var Power = Math.pow(10, DecDigits)
	var Num = Math.round(Value * Power) / Power;

	return Num;
}

//-------------------------------------------------------------------------
function Num(Value)
{
  var Num = 0;
	var sNum = "0";
	var fDec = false;
	var Dec = 1.0;
	var fNeg = false;
	var i;
	var Chr;


	if (Value)
	{	
		for (i = 0; i < Value.length; i++)
		{
			Chr = Value.charAt(i);
			switch (Chr)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					sNum += Chr;
					break;
					
				case '.':
					if (!fDec)
					{
						sNum += ".";
					}
					fDec = true;
					break;
					
				case '-':
					fNeg = true;
					break;
			}
		}
	}		
	
	Num = parseFloat(sNum);
	if (fNeg)
	{
		Num = -Num;
	}
	return Num;
}

//-------------------------------------------------------------------------
function NumYearsMonths(NumMonths)
{
	var str = "";
	var Years = Math.floor(NumMonths / 12);
	var Months = NumMonths % 12;
	
	if (Years > 0)
	{
		str = Years + " Yr" + (Years == 1 ? "" : "s");
	}
	if (Months > 0)
	{
		str += " " + Months + " Mth" + (Months == 1 ? "" : "s");
	}

	if (str == "")
	{
		str = "&nbsp;";
	}
	
	return str;
}

//-------------------------------------------------------------------------
function SetupBrowser()
{
	fIE  = (document.all ? true : false);
	fDOM = (document.getElementById ? true : false);
	
	cnShow = 'visible';
	cnHide = 'hidden';
}

//-------------------------------------------------------------------------
function ShowLayer(Layer, BesideElement, LeftMargin, HideElements)
{	
	var LayerStyle = GetStyle(Layer);
	LayerStyle.visibility = cnShow;

	if (BesideElement)
	{
		LayerStyle.top = GetY(BesideElement) + BesideElement.offsetHeight;
		if (LeftMargin)
		{
			LayerStyle.left = LeftMargin;
		}
		else
		{
			LayerStyle.left = GetX(BesideElement);
		}
	}

	if (fIE && HideElements)
	{
		for (var i = 0; i < HideElements.length; i++)
		{
			HideElement(HideElements[i]);
		}
	}
}

//-------------------------------------------------------------------------
function HideLayer(Layer, ShowElements)
{
	var LayerStyle = GetStyle(Layer);
	LayerStyle.visibility = cnHide;

	if (fIE && ShowElements)
	{
		for (var i = 0; i < ShowElements.length; i++)
		{
			ShowElement(ShowElements[i]);
		}
	}
}

//-------------------------------------------------------------------------
function ShowElement(ElementName)
{
	var Style = GetStyle(ElementName);
	if (Style)
	{
		Style.visibility = cnShow;
	}
}

//-------------------------------------------------------------------------
function HideElement(ElementName)
{
	var Style = GetStyle(ElementName);
	if (Style)
	{
		Style.visibility = cnHide;
	}
}

//-------------------------------------------------------------------------
function ShowElementList(ElementNameList)
{
	if (ElementNameList)
	{
		var aElementName = ElementNameList.split(",");	
		for (var i = 0; i < aElementName.length; i++)
		{
			ShowElement(Trim(aElementName[i]));
		}
	}
}

//-------------------------------------------------------------------------
function HideElementList(ElementNameList)
{
	if (ElementNameList)
	{
		var aElementName = ElementNameList.split(",");
		for (var i = 0; i < aElementName.length; i++)
		{
			HideElement(Trim(aElementName[i]));
		}
	}
}

//-------------------------------------------------------------------------
function GetStyle(ElementName) 
{
	var Style = null;

	if (fDOM) 	
	{
		var e = GetDocObj(ElementName);
		if (e)
		{
			Style = e.style;
		}
	}
	else 
	{
		Style = (fIE ? document.all[Name].style : document.layers[Name]); 
	}

	return Style;
}

//-------------------------------------------------------------------------
function GetX(Element)
{
	return (Element.x ? Element.x : GetPos(Element, "Left"));
}

//-------------------------------------------------------------------------
function GetY(Element) 
{
	return (Element.y ? Element.y : GetPos(Element, "Top"));
}

//-------------------------------------------------------------------------
function GetPos(Element, Which)
{
	var iPos = 0;
	while (Element != null)
	{
		iPos += Element["offset" + Which];
		Element = Element.offsetParent;
	}

	return iPos;
}

//-------------------------------------------------------------------------
function SetDisplay(ElementName, fDisplay)
{
	var Style = GetStyle(ElementName);
	if (Style)
	{
		Style.display = (fDisplay ? "" : "none");
	}
}

//-------------------------------------------------------------------------
function SetDisplayList(ElementNameList, fDisplay)
{
	if (ElementNameList)
	{
		var aElementName = ElementNameList.split(",");
		for (var i = 0; i < aElementName.length; i++)
		{
			SetDisplay(Trim(aElementName[i]), fDisplay);
		}
	}
}

//-------------------------------------------------------------------------
function DisplayOn(ElementName, fIcon)
{
	var e = GetDocObj(ElementName);
	if (e)
	{
		e.className = "DisplayOn";

		if (arguments.length > 1 && fIcon)
		{
			e.className += "Icon";
		}
	}
}

//-------------------------------------------------------------------------
function DisplayOff(ElementName, fIcon)
{
	var e = GetDocObj(ElementName);
	if (e)
	{
		e.className = "DisplayOff";
	
		if (arguments.length > 1 && fIcon)
		{
			e.className += "Icon";
		}
	}
}

//-------------------------------------------------------------------------
function IsDisplayOn(ElementName)
{
	var fOn = false;
	
	var e = GetDocObj(ElementName);
	if (e)
	{
		fOn = (e.className == "DisplayOn");
	}
	
	return fOn;
}

//-------------------------------------------------------------------------
function SetClass(ElementName, ClassName)
{
	var e = GetDocObj(ElementName);
	if (e)
	{
		e.className = ClassName;
	}
}

//-------------------------------------------------------------------------
function SetTitle(ElementName, Title)
{
	var e = GetDocObj(ElementName);
	if (e)
	{
		e.title = Title;
	}
}

//-------------------------------------------------------------------------
function SetTarget(Form, Target)
{
	var e = GetDocObj(Form);
	if (e)
	{
		e.target = Target;
	}
}

var PrevCursor = "default";
//-------------------------------------------------------------------------
function CursorWait()
{
	PrevCursor = document.body.style.cursor;
	document.body.style.cursor = "wait";
}

//-------------------------------------------------------------------------
function CursorReset()
{
//	document.body.style.cursor = PrevCursor;
	document.body.style.cursor = "default";
}

//-------------------------------------------------------------------------
function ShowAddlFlds(Cnt)
{
	if (Cnt)
	{
		for (var i = 1; i <= Cnt; i++)
		{
			DisplayOn('AddlFlds' + i);
		}
	}
	else
	{
		DisplayOn('AddlFlds');
	}

	DisplayOff('AddlFldsShowIcon', true);
	DisplayOn('AddlFldsHideIcon', true);
	HideLayer('AddlFldsShowIconHelp');
}

//-------------------------------------------------------------------------
function HideAddlFlds(Cnt)
{
	if (Cnt)
	{
		for (var i = 1; i <= Cnt; i++)
		{
			DisplayOff('AddlFlds' + i);
		}
	}
	else
	{
		DisplayOff('AddlFlds');
	}

	DisplayOn('AddlFldsShowIcon', true);
	DisplayOff('AddlFldsHideIcon', true);
	HideLayer('AddlFldsHideIconHelp');
}

//-------------------------------------------------------------------------
function ClassOver(Element)
{
	Element.className += "Over";
}

//-------------------------------------------------------------------------
function ClassOut(Element)
{
	var i = Element.className.indexOf("Over");
	Element.className = Element.className.substring(0, i);
} 

var fSkipOpen = false;

//-------------------------------------------------------------------------
function OpenPage(Page)
{
	if (!fSkipOpen)
	{
		try
		{
			window.location.href = Page;
//			window.navigate(Page);
		}
		catch (err)
		{
			//alert("there was an error " + err.description);
		}
	}
	
	fSkipOpen = false;
}

//-------------------------------------------------------------------------
function IsSafari()
{
	var Agent = navigator.userAgent;
	var fSafari 
		= (Agent.indexOf("Safari") > 0 &&
			 Agent.indexOf("Macintosh") > 0);
	return fSafari;
}		

var HighLightPrevID = "";
//-------------------------------------------------------------------------
function HighLight(ID)
{
	if (HighLightPrevID != "")
	{
		SetClass(HighLightPrevID , "HighLight");		
	}
	
	SetClass(ID, "HighLightOn");
	
	HighLightPrevID = ID;
}

//---------------------------------------------------------------------------
function ValidateCurrency(Element, ZeroMode)
{
	if (arguments.length < 2)
	{
		ZeroMode = "";
	}
	
	Element.value = FmtDollar(Num(Element.value), ZeroMode);
}

//---------------------------------------------------------------------------
function ValidatePct(Element, DecDigits, ZeroMode)
{
	if (arguments.length < 2)
	{
		DecDigits = 2;
	}
	
	if (arguments.length < 3)
	{
		ZeroMode = "";
	}
	
	Element.value = FmtPct(Num(Element.value), DecDigits, ZeroMode);
}

//---------------------------------------------------------------------------
function ValidateNum(Element)
{
	Element.value = FmtNum(Num(Element.value), 0);
}

//---------------------------------------------------------------------------
function ValidateRno(Element)
{
	Element.value = Num(Element.value);
}

//---------------------------------------------------------------------------
function ValidateDate(Element)
{
	var fOk = true;
	
	if (Element.value != "")
	{
		var Dt = new Date(Element.value);
		if (isNaN(Dt))
		{
			fOk = false;
		}
		else
		{
			var a = Element.value.split("/");
			
			if (a.length >= 3)
			{
				var Month = Num(a[0]);
				var Day   = Num(a[1]);
				var Year  = Num(a[2]);
				
				if (Month < 1 || Month > 12 ||
					Day   < 1 || Day   > 31 ||
					Year  < 1 || Year  > 3000)
				{
					fOk = false;
				}
				else
				{
					var Days = Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
					if (Year % 4 == 0)
					{
						Days[1]++;
					}
					
					if (Day > Days[Month - 1])
					{
						fOk = false;
					}
				}

				//check for a 2 digit year and fix to a 4 digit year
				if (a[2].length <= 2)
				{
					Dt.setFullYear(Dt.getFullYear() + 100);
				}
			}
		}
		
		if (fOk)
		{
			Element.value = Dt.getMonth() + 1 + "/" + Dt.getDate() + "/" + Dt.getFullYear();
		}
		else
		{
			alert("Invalid date.");
		}
	}
}

//---------------------------------------------------------------------------
function ValidateDateField(FieldName)
{
	var Field = GetDocObj(FieldName);
	
	if (Field)
	{
		ValidateDate(Field);
	}
}

//---------------------------------------------------------------------------
function ValidateTime(Element)
{
	var fValid = true;
	var Hours   = 0;
	var Minutes = 0;
	
	if (Element.value != "")
	{
		var Now = new Date();
		var Dt = new Date(Now.getMonth() + 1 + "/" + Now.getDate() + "/" + Now.getFullYear + " " + Element.value);
		if (isNaN(Dt))
		{
			var Tm  = Element.value.toLowerCase();
			Hours   = Num(Tm);
			Minutes = 0;
			
			var i = Tm.indexOf(":");
			if (i >= 0)
			{
				Hours   = Num(Tm.substr(0, i));
				Minutes = Num(Tm.substr(i + 1));
			}

			if (Tm.indexOf("a") < 0)	// 'a' not there
			{
				if (Hours > 0 && Hours < 12)
				{
					Hours += 12;
				}
			}
			else
			{							// 'a' is there
				if (Hours == 12)
				{
					Hours -= 12;
				}
			}
			
			Dt = new Date("1/1/1 " + Hours + ":" + Minutes);
			if (isNaN(Dt))
			{
				alert("Invalid time.");
				fValid = false;
			}
		}
		else
		{
			Hours   = Dt.getHours();
			Minutes = Dt.getMinutes();
		}
		
		if (fValid)
		{
			var AmPm = "";
			if (Hours < 12)
			{
				AmPm = "AM";
				if (Hours == 0)
				{
					Hours = 12;
				}
			}
			else
			{
				AmPm = "PM";
				if (Hours > 12)
				{
					Hours -= 12;
				}
			}
			
			Element.value = Hours + ":" + (Minutes < 9 ? "0" : "") + Minutes + " " + AmPm;
		}
	}	
}

//----------------------------------------------------------------------------
function SearchList(SearchBox, ListBox)
{
	// make a copy of the options
	var jqListBox = $(ListBox);
	var Options = $("<div id=\"options" + jqListBox.attr("id") + "\"></div>").hide().append($("option", jqListBox).clone());
	jqListBox.after(Options);
	Options = $("option", Options);

	FindSearchList($(SearchBox).val(), jqListBox, Options);

	$(SearchBox).keyup(function ()
	{
		FindSearchList($(this).val(), jqListBox, Options);
	});
}

//----------------------------------------------------------------------------
function FindSearchList(SearchValue, ListBox, Options)
{
	if (SearchValue.length == 0)
	{
		ListBox.children().detach();
		ListBox.append(Options);
	}
	else
	{
		ListBox.children().detach();
		var SearchValue = SearchValue.toLowerCase();
		Options.each(function ()
		{
			if ($(this).html().toLowerCase().indexOf(SearchValue) >= 0)
			{
				ListBox.append($(this).clone());
			}
		});

		// for some reason on the Job page the select doesn't always refresh automatically
		// so we need to help it out
		var urlParts = window.location.pathname.split("/");
		if (urlParts[urlParts.length - 1] == "Job.aspx")
		{
			ListBox.hide();
			setTimeout("$('#" + ListBox.prop("id") + "').show()", 0);
		}
	}
}

var fDirty = false;
var fSkipDirty = false;

//---------------------------------------------------------------------------
function SetDirty()
{
	fDirty = true;
	iSetStr("txtDirty", "true");
}

//---------------------------------------------------------------------------
function ClearDirty()
{
	fDirty = false;
	iSetStr("txtDirty", "false");
}

//---------------------------------------------------------------------------
function SkipDirty()
{
	fSkipDirty = true;
}

//---------------------------------------------------------------------------
function CheckDirty()
{
	var rc = undefined;

	if (fDirty && !fSkipDirty)
	{
		rc = "Some values have changed and will be lost unless you save them first.\n\nTo save your changes, Cancel now then click Save or Update.";
	}
	fSkipDirty = false;
	
	return rc;
}

//---------------------------------------------------------------------------
function HideDirty()
{
	fDirty = false;
}

//---------------------------------------------------------------------------
function RestoreDirty()
{
	fDirty = (iGetStr("txtDirty") == "true");
}

//-----------------------------------------------------------------------------
function FocusOnNextField(FieldName)
{
	// find the next field in the form
	var f = GetDocObj(FieldName);
	
	if (f)
	{
		for (var i = 0; i < f.form.elements.length; i++)
		{
			var e = f.form.elements[i];
			if (e == f)
			{
				var fFound = false;
				
				// find the next input element and set the focus
				for (i = i + 1; i < f.form.elements.length; i++)
				{
					var e = f.form.elements[i];
					if (e.type)
					{
						switch (e.type)
						{
							case "button":
							case "checkbox":
							case "file":
							case "password":
							case "radio":
							case "reset":
							case "submit":
							case "text":
							case "textarea":
								if (!e.disabled)
								{
									e.focus();
									fFound = true;
								}
								break;
						}
					}
					
					if (fFound)
					{
						break;
					}
				}
				break;
			}
		}
	}
}

//-----------------------------------------------------------------------------
function Qtips(Selector, options)
{
	$(Selector).qtip(
	{
		style:
		{
			name: "cream",
			width:
			{
				max:  options && options.width || 450
			},
			border:
			{
				radius: 5
			}
		},
		position:
		{
		    corner:
			{
			    target: options && options.target || "bottomLeft",
			    tooltip: options && options.tooltip || "topRight"
			},
		    adjust:
            {
                x: options && options.x || 0,
                y: options && options.y || 0,
            }
		}
	});
	$(Selector).data("qtipSet", true);
}

//-----------------------------------------------------------------------------
function QtipUpdate(Selector, Tip)
{
	if ($(Selector).data("qtipSet"))
	{
		$(Selector).qtip("api").updateContent(Tip);
	}
	else
	{
		$(Selector).attr("title", Tip);
		Qtips(Selector);
	}
}

//-----------------------------------------------------------------------------
var urlParms;
(window.onpopstate = function ()
{
	var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);

	urlParms = {};
	while (match = search.exec(query))
		urlParms[decode(match[1])] = decode(match[2]);
})();
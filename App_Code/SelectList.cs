using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for SelectList.
	/// </summary>
	public class SelectList
	{
		private string sName;
		private TextBox txtBox;
		private Image imgButton;
		private string sNextField;
		private bool fAutoPostBack;
		private bool fAutoWidth;
		private int nHeight;
		private string sStyleClass;
		private string sSelectStyleClass;
		private StringCollection Values;

		private static SelectList[] aSL = null;

		public SelectList(
			String Name, 
			ref TextBox txtBox)
		{
			this.sName             = Name;
			this.txtBox            = txtBox;
			this.sNextField        = null;
			this.fAutoPostBack     = false;
			this.fAutoWidth        = true;
			this.nHeight           = 0;
			this.sStyleClass       = "SelectList";
			this.sSelectStyleClass = "SelectListCurr";
			this.Values            = new StringCollection();

			AddThisInstance();
			//AddEvents();
		}

		public String Name
		{
			get
			{
				return sName;
			}
			set
			{
				sName = value;
			}
		}

		public String NextField
		{
			get
			{
				return sNextField;
			}
			set
			{
				sNextField = value;
			}
		}

		public bool AutoPostBack
		{
			get
			{
				return fAutoPostBack;
			}
			set
			{
				fAutoPostBack = value;
			}
		}

		public bool AutoWidth
		{
			get
			{
				return fAutoWidth;
			}
			set
			{
				fAutoWidth = value;
			}
		}

		public int Height
		{
			get
			{
				return nHeight;
			}
			set
			{
				nHeight = value;
			}
		}

		public String StyleClass
		{
			get
			{
				return sStyleClass;
			}
			set
			{
				sStyleClass = value;
			}
		}

		public String SelectStyleClass
		{
			get
			{
				return sSelectStyleClass;
			}
			set
			{
				sSelectStyleClass = value;
			}
		}

		public static void Clear()
		{
			SelectList.aSL = null;
		}

		private void AddThisInstance()
		{
			if (SelectList.aSL == null)
			{
				SelectList.aSL = new SelectList[1];
				SelectList.aSL[0] = this;
			}
			else
			{
				SelectList[] aNew = new SelectList[SelectList.aSL.Length + 1];
				SelectList.aSL.CopyTo(aNew, 0);
				aNew[SelectList.aSL.Length] = this;
				SelectList.aSL = aNew;
			}
		}

		private void AddEvents()
		{
			txtBox.Attributes.Add("onFocus", txtBox.Attributes["onFocus"] + "sl" + sName + ".OnFocus();");
			txtBox.Attributes.Add("onKeyDown", "return sl" + sName + ".ValueKeyDown(event);");
			txtBox.Attributes.Add("onKeyUp", "sl" + sName + ".ValueKeyUp();");
			txtBox.Attributes.Add("onBlur",	"sl" + sName + ".OnBlur();");	//"dBug('onBlur " + sName + "');setTimeout('sl" + sName + ".Hide();', 200);");

			/*
			Control Parent = txtBox.Parent;
			
			foreach (Control Sibling in Parent.Controls)
			{
				TextBox TextBox = new TextBox();
				if (Sibling.GetType() == TextBox.GetType() && 
					Sibling.Visible)
				{
					WebControl Sib = (WebControl)Sibling;
					if (Sib != txtBox)
					{
						//Sib.Attributes.Add("onFocus", "CloseAllSLs();" + Sib.Attributes["onFocus"]);
					}
				}
			}
			*/
		}

		public void ImageButton(Image imgButton)
		{
			this.imgButton = imgButton;
			//imgButton.Attributes.Add("onClick", imgButton.Attributes["onClick"] + "CloseAllSLs();sl" + sName + ".ShowValues();");
			imgButton.Attributes.Add("onClick", "sl" + sName + ".ButtonClicked();");
		}

		public void ClearValues()
		{
			Values.Clear();
		}

		public void AddValue(String Value)
		{
			if (Value != "")
			{
				Values.Add(Value);
			}
		}

		public void AddDBValues(DB db, String Sql)
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				String Value = DB.Str(r[0]);
				if (Value != "")
				{
					bool fFound = false;
					for (int i = 0; i < Values.Count; i++)
					{
						if (Values[i] == Value)
						{
							fFound = true;
							break;
						}
					}

					if (!fFound)
					{
						Values.Add(Value);
					}
				}
			}
		}

		public static SelectList Find(String Name)
		{
			SelectList slFound = null;

			if (SelectList.aSL != null)
			{
				for (int i = 0; i < SelectList.aSL.Length; i++)
				{
					if (SelectList.aSL[i].Name == Name)
					{
						slFound = SelectList.aSL[i];
						break;
					}
				}
			}

			return slFound;
		}

		public static String JavaScript()
		{
			int i;

			String Html = 
				"<script language=\"javascript\" type=\"text/javascript\">\n" +
				"\n" +
				"//-----------------------------------------------------------------------------\n" + 
				"function slCloseAll(NotThisOne)\n" + 
				"{\n";

			if (SelectList.aSL != null)
			{
				for (i = 0; i < SelectList.aSL.Length; i++)
				{
					SelectList sl = SelectList.aSL[i];
					sl.AddEvents();

					Html += 
						"	if (NotThisOne != \"" + sl.sName + "\")\t{ sl" + sl.sName + ".fHide = true; sl" + sl.sName + ".Hide(); }\n";
				}
			}
			Html += 
				"}\n" + 
				"\n" + 
				"\n";

			if (SelectList.aSL != null)
			{
				for (i = 0; i < SelectList.aSL.Length; i++)
				{
					SelectList sl = SelectList.aSL[i];
					string OnChange = sl.txtBox.Attributes["onChange"];
					
					Html += 
						"var sl" + sl.sName + " = new SelectList(" + 
						"\"" + sl.sName + "\", " +
						"\"" + sl.txtBox.ID + "\", " + 
						(sl.sNextField != null ? "\"" + sl.sNextField + "\"" : "null") + ", " +
						(OnChange != null ? "\"" + OnChange + "\"" : "null") + ", " +
						(sl.fAutoPostBack ? "true" : "false") + ", " + 
						(sl.fAutoWidth ? "true" : "false") + ", " + 
						"\"" + sl.sStyleClass + "\", " +
						"\"" + sl.sSelectStyleClass + "\");\n";
				}
			}
			Html += 
				"\n" + 
				"</script>\n";

			return Html;
		}

		public static String DefValues()
		{
			String Html = "";

			if (SelectList.aSL != null)
			{
				for (int i = 0; i < SelectList.aSL.Length; i++)
				{
					SelectList sl = SelectList.aSL[i];
					Html += 
						"<div id=\"div" + sl.sName + "\" class=\"" + sl.StyleClass + "\" style=\"border: black 1px solid; padding: 3px; padding-top: 1px; overflow: auto; cursor: default; display: none; position: absolute;" + 
						(sl.nHeight > 0 ? " height: " + sl.nHeight + "px;" : "") + "\" onFocus=\"sl" + sl.sName + ".DivFocus();\">\n" +
						"	<table border=\"0\" id=\"tbl" + sl.sName + "Values\" cellpadding=\"0\" cellspacing=\"0\"" + (sl.fAutoWidth ? "" : " width=\"100%\"") + ">\n" +
						"		<tr><td><img id=\"img" + sl.sName + "Width\" width=\"1\" height=\"1\" src=\"Images/Space.gif\" alt=\"\" /></td></tr>\n";

					if (sl.Values.Count > 0)
					{

						foreach (String Value in sl.Values)
						{
							Html += 
								"		<tr><td onMouseDown=\"sl" + sl.sName + ".Select(this);\">" + Value + "</td></tr>\n";
						}
					}

					Html += 
						"	</table>\n" + 
						"</div>\n";
				}
			}

			return Html;
		}

		public string jsArray()
		{
			StringBuilder sb = new StringBuilder();

			foreach (String Value in Values)
			{
				sb.Append(string.Format("\"{0}\", ", Value.Replace("\"", "\\\"")));
			}

			return sb.ToString();
		}
	}
}

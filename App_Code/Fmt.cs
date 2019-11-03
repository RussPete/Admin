using System;
using System.Web.UI.WebControls;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for Fmt.
	/// </summary>
	public class Fmt
	{
		public Fmt()
		{
		}

		public static string Num(Int32 Num)
		{
			return String.Format("{0:#,###,###,##0}", Num);
		}

		public static string Num(Int32 Num, bool fShowZero)
		{
			string Value = "";

			if (Num != 0 || fShowZero)
			{
				Value = String.Format("{0:#,###,###,##0}", Num);
			}

			return Value;
		}

		// shows as few decimals digits as possible
		public static string Num(Decimal Num, int MaxDecDigits)
		{
			return Fmt.Num(Num, MaxDecDigits, true);
		}

		// shows as few decimals digits as possible
		public static string Num(Decimal Num, int MaxDecDigits, bool fShowZero)
		{
			string Value = string.Empty;
			if (!(Num == 0 && !fShowZero))
			{
				Value = Num.ToString("#,###,##0." + new string('#', MaxDecDigits));
			}

			return Value;
		}

		public static string Dec(Decimal Num)
		{
			return String.Format("{0:#,###,##0.00}", Num);
		}

		public static string Dollar(Decimal Num)
		{
			return String.Format("{0:$#,###,##0.00}", Num);
		}

		public static string Dollar(Decimal Num, bool fShowZero)
		{
			string Value = "";

			if (Num != 0 || fShowZero)
			{
				Value = String.Format("{0:$#,###,###,##0.00}", Num);
			}

			return Value;
		}

		public static string Pct(Decimal Num)
		{
			return String.Format("{0:##0}%", Num);
		}

		public static string Pct(Decimal Num, bool fShowZero)
		{
			string Value = "";

			if (Num != 0 || fShowZero)
			{
				Value = Pct(Num);
			}

			return Value;
		}

		public static string Pct(Decimal Num, int nDecDigits)
		{
			string Fmt = "##0";
			if (nDecDigits > 0)
			{
				Fmt += ".";
				for (int i = 0; i < nDecDigits; i++)
				{
					Fmt += "0"; 
				}
			}

			return String.Format("{0:" + Fmt + "}%", Num);
		}

		public static string Pct(Decimal Num, int nDecDigits, bool fShowZero)
		{
			string Value = "";

			if (Num != 0 || fShowZero)
			{
				Value = Pct(Num, nDecDigits);
			}

			return Value;
		}

		public static string Dt(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:MM/dd/yyyy}", DtTm);
			}

			return Value;
		}

		public static string DtTm(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:MM/dd/yyyy HH:mm}", DtTm);
			}

			return Value;
		}

		public static string DtTmSec(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:MM/dd/yyyy HH:mm:ss}", DtTm);
			}

			return Value;
		}

		public static string DtTm12Hr(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:MM/dd/yyyy h:mm tt}", DtTm);
			}

			return Value;
		}

		public static string DtTm12HrSec(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:MM/dd/yyyy h:mm:ss tt}", DtTm);
			}

			return Value;
		}

		private static string[] Nth = { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };

		public static string DtNth(DateTime Dt)
		{
			return string.Format("{0:dddd, MMMM d}{1}", Dt, Nth[Dt.Day / 10 == 1 ? 0 : Dt.Day % 10]);
		}

		public static string Tm(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:hh:mm}", DtTm);
			}

			return Value;
		}

		public static string TmSec(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:hh:mm:ss}", DtTm);
			}

			return Value;
		}

		public static string Tm12Hr(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue )
			{
				Value = String.Format("{0:h:mm tt}", DtTm);
			}

			return Value;
		}

		public static string Tm12HrSec(DateTime DtTm)
		{
			string Value = "";

			if (DtTm != DateTime.MinValue)
			{
				Value = String.Format("{0:h:mm:ss tt}", DtTm);
			}

			return Value;
		}

		public static readonly string[] DaysOfWeek = 
		{
			"Sunday",
			"Monday", 
			"Tuesday", 
			"Wednesday", 
			"Thursday", 
			"Friday",
			"Saturday"
		};

		public static string DayOfWeek(DateTime DtTm)
		{
			string Day = "";
			

			if (DtTm != DateTime.MinValue)
			{
				Day = DaysOfWeek[(int)DtTm.DayOfWeek];
			}

			return Day;
		}

		public static void SetRadioButtonList(ref RadioButtonList rb, string Value)
		{
			for (int i = 0; i < rb.Items.Count; i++)
			{
				ListItem Item = rb.Items[i];
				if (Item.Value == Value)
				{
					rb.SelectedIndex = i;
					break;
				}
			}
		}

        public static string YesNo(bool f)
        {
            return (f ? "Yes" : "No");
        }
	}
}

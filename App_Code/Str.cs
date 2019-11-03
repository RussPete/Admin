using System;
using System.Text;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for Str.
	/// </summary>
	public class Str
	{
		public Str()
		{
		}

        public static string NullStr(string Str)
        {
            string Value = Str.Trim();
            return (Value.Length == 0 ? null : Value);
        }

		public static Int32 Num(String Str)
		{
			Int32 Value = 0;

			if (Str != "")
			{
				try
				{
					Value = Convert.ToInt32(Str);
				}
				catch (Exception Ex)
				{
					Ex.GetType();

					Char[] aChr = Str.ToCharArray();
					bool fNeg = false;

					foreach (Char Chr in aChr)
					{
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
								Value = Value * 10 + Chr - '0'; 
								break;

							case '-':
								fNeg = true;
								break;
						}
					}

					if (fNeg)
					{
						Value = -Value;
					}
				}
			}

			return Value;
		}

        public static Int32? NullNum(String Str)
        {
            Int32? Value = null;

            if (Str.Trim().Length > 0)
            {
                Value = Num(Str);
            }

            return Value;
        }

		public static decimal Dec(String Str)
		{
			decimal Value = 0;

			if (Str != "")
			{
				try
				{
					Value = Convert.ToDecimal(Str);
				}
				catch (Exception Ex)
				{
					Ex.GetType();

					Char[] aChr = Str.ToCharArray();
					bool fNeg = false;
					bool fDec = false;
					decimal DecPower = 1;

					foreach (Char Chr in aChr)
					{
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
								if (!fDec)
								{
									Value = Value * 10 + Chr - '0'; 
								}
								else
								{
									DecPower *= 0.1m;
									Value = Value + DecPower * (Chr - '0'); 
								}
								break;

							case '-':
								fNeg = true;
								break;

							case '.':
								fDec = true;
								break;
						}
					}

					if (fNeg)
					{
						Value = -Value;
					}
				}
			}

			return Value;
		}

        public static Int64 Int64(String Str)
        {
            Int64 Value = 0;

            if (Str != "")
            {
                try
                {
                    Value = Convert.ToInt32(Str);
                }
                catch (Exception Ex)
                {
                    Ex.GetType();

                    Char[] aChr = Str.ToCharArray();
                    bool fNeg = false;

                    foreach (Char Chr in aChr)
                    {
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
                                Value = Value * 10 + Chr - '0';
                                break;

                            case '-':
                                fNeg = true;
                                break;
                        }
                    }

                    if (fNeg)
                    {
                        Value = -Value;
                    }
                }
            }

            return Value;
        }

        public static DateTime DtTm(object Obj)
		{
			DateTime Value = DateTime.MinValue;

			if (Obj != DBNull.Value)
			{
				string sValue = Obj as string;
				if (sValue != null)
				{
					Value = Str.DtTm(sValue);
				}
			}

			return Value;
		}

		public static DateTime DtTm(String Str)
		{
			DateTime Value = DateTime.MinValue;

			if (Str != "")
			{
				try
				{
					Value = Convert.ToDateTime(Str);
				}
				catch (Exception Ex)
				{
					Ex.GetType();

					int Year  = 0;
					int Month = 0;
					int Day   = 0;
					int Hour  = 0;
					int Min   = 0;
					int Sec   = 0;

					Char[] Separators = {' ', '/', ':', '.'};
					Str = Str.Trim().ToUpper();
					String[] Parts = Str.Split(Separators);
					for (int iParts = 0; iParts < Parts.Length; iParts++)
					{
						switch (iParts)
						{
							case 0: 
								Month = Num(Parts[iParts]);
								break;

							case 1: 
								Day = Num(Parts[iParts]);
								break;

							case 2: 
								Year = Num(Parts[iParts]);
								break;

							case 3: 
								Hour = Num(Parts[iParts]);
								break;

							case 4: 
								Min = Num(Parts[iParts]);
								break;

							case 5: 
								Sec = Num(Parts[iParts]);
								break;
						}
					}

					if (Hour > 0 && Hour <= 12)
					{
						String Last = Parts[Parts.Length - 1];
						if (Last.IndexOf('P') > -1)
						{
							Hour += 12;
						}
					}

					if (Year < 99)	{ Year += 2000;	}
					if (Month == 0)	{ Month = 1; }
					if (Month > 12)	{ Month = 12; }
					if (Day == 0)	{ Day = 1; }
					if (Day > 31)	{ Day = 31; }
					if (Month == 2 && 
						Day > 29)	{ Day = 29; }
					if (Hour > 24)	{ Hour = 24; }
					if (Min > 59)	{ Min = 59; }
					if (Sec > 59)	{ Sec = 59; }

					if (Year > 0 && Month > 0 && Day > 0)
					{
						try 
						{
							Value = new DateTime(Year, Month, Day, Hour, Min, Sec);
						}
						catch (Exception Ex1)
						{
							Ex1.GetType();

							Day--;
							try 
							{
								Value = new DateTime(Year, Month, Day, Hour, Min, Sec);
							}
							catch (Exception Ex2)
							{
								Ex2.GetType();
							}
						}
					}
				}
			}

			return Value;
		}

        public static DateTime DtTmEnd(String Str)
        {
            DateTime Value = DtTm(Str);
            if (Value == DateTime.MinValue)
            {
                Value = DateTime.MaxValue;
            }

            return Value;
        }

        public static bool Bool(string Str)
		{
			bool fBool = false;

			if (Str != null)
			{
				Str = Str.ToLower();
				fBool = (Str == "true" || Str == "1");
			}

			return fBool;
		}

		public static decimal Fract(string Str)
		{
			decimal Value = 0;

			// look for a whole number and a fraction
			string[] aStr = Str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (aStr.Length > 1)
			{
				// got the whole number
				Value = Dec(aStr[0]);
				Str = aStr[1];
			}

			// look for a fraction
			aStr = Str.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (aStr.Length > 1)
			{
				decimal Denominator = Dec(aStr[1]);
				if (Denominator != 0)
				{
					Value += Dec(aStr[0]) / Denominator;
				}
			}
			else
			{
				// no fraction, just a decimal
				Value += Dec(Str);
			}

			return Value;
		}

		public static string ShowFract(decimal Dec)
		{
			return ShowFract(Dec, 4, true);
		}

		public static string ShowFract(decimal Dec, int MaxDecDigits)
		{
			return ShowFract(Dec, MaxDecDigits, true);
		}

		public static string ShowFract(decimal Dec, bool fShowZero)
		{
			return ShowFract(Dec, 4, fShowZero); 
		}

		public static string ShowFract(decimal Dec, int MaxDecDigits, bool fShowZero)
		{
			string Value = Fmt.Num(Dec, MaxDecDigits); //Dec.ToString("#.#####");
			if (Value == "0" && Dec > 0)
			{
				Value = "~0";
			}

			decimal WholeNumber = Math.Floor(Dec);
			Dec -= WholeNumber;

			if (WholeNumber == 0 && Dec == 0)
			{
				if (!fShowZero)
				{
					Value = string.Empty;
				}
			}
			else if (Dec == 0.3333m)
			{
				Value = string.Format("{0:#} 1/3", WholeNumber);
			}
			else if (Dec == 0.6667m)
			{
				Value = string.Format("{0:#} 2/3", WholeNumber);
			}
			else
			{
				for (int Denominator = 1; Denominator <= 1000; Denominator++)
				{
					int Numerator = (int)Math.Round(Dec * Denominator, 0);
					if (Math.Abs(Dec - 1.0m * Numerator / Denominator) < 0.000001m)
					{
						if (Denominator > 1)
						{
							if (Denominator <= 8)
							{
								Value = string.Format("{0:#} {1}/{2}", WholeNumber, Numerator, Denominator);
							}
						}
						break;
					}
				}
			}

			return Value.Trim();
		}

		public static string js(string str)
		{
			return str.Replace("\"", "\\\"");
		}

		public static string Append(string Source, string Join, string Str)
		{
			string NewStr = Source;

			if (Source.Length > 0 && Str.Length > 0)
			{
				NewStr += Join + Str;
			}
			else if (Str.Length > 0)
			{
				NewStr = Str;
			}

			return NewStr;
		}

		public static string Left(string Str, int Len)
		{
			// get the min
			Len = (Len > Str.Length ? Len : Str.Length);
			return Str.Substring(0, Len);
		}

		public static string Right(string Str, int Len)
		{
			// get the min
			Len = (Len < Str.Length ? Len : Str.Length);
			return Str.Substring(Str.Length - Len, Len);
		}

		public static string Join(string[] Values, string JoinBy)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string Str in Values)
			{
				if (Str.Length > 0)
				{
					sb.Append(Str).Append(JoinBy);
				}
			}
			if (sb.Length > 0)
			{
				sb.Remove(sb.Length - 1, 1);
			}

			return sb.ToString();
		}
	}
}

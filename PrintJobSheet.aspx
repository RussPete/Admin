<%@ Page AutoEventWireup="true" CodeFile="PrintJobSheet.aspx.cs" Inherits="PrintJobSheet"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<HEAD>
		<title><%= Globals.g.Company.Name %> - PrintJobSheet</title> 
		<!-- PrintJobSheet.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<style>
		.CustTitle { WIDTH: 80px }
		</style>
	</HEAD>
	<body class="JobBody">
		<form id="form" method="post" runat="server">
			<table cellspacing="0" cellpadding="0" align="center" border="0" height="100%">
				<tr>
					<td colspan="3">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td width="30%">Printed:
									<asp:label id="lblPrinted" runat="server"></asp:label></td>
								<td class="JobTitle" align="center">Job #
									<asp:label id="lblJobRno" runat="server"></asp:label></td>
								<td align="right" width="30%">Status:
									<asp:label id="lblStatus" runat="server"></asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px; WIDTH: 400px">
						<table cellspacing="0" cellpadding="0" border="0" height="100%" width="100%">
							<tr>
								<td valign="top">
									<table cellspacing="0" cellpadding="0" border="0" align="center">
										<tr>
											<td>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="1" src="Images/Space.gif" alt="" class="CustTitle"></td>
														<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
														<td></td>
													</tr>
													<tr>
														<td align="right">Customer</td>
														<td></td>
														<td><asp:label id="lblCustomer" runat="server" cssclass="JobSubTitle"></asp:label></td>
													</tr>
													<tr>
														<td align="right">Contact</td>
														<td></td>
														<td><asp:label id="lblName" runat="server" cssclass="JobItem"></asp:label></td>
													</tr>
													<%	if (lblPhone.Text.Length > 0)
													{ %>
													<tr>
														<td align="right">Phone #</td>
														<td></td>
														<td><asp:label id="lblPhone" runat="server"></asp:label></td>
													</tr>
													<%	}
													if (lblCell.Text.Length > 0)
													{ %>
													<tr>
														<td align="right">Cell #</td>
														<td></td>
														<td><asp:label id="lblCell" runat="server"></asp:label></td>
													</tr>
													<%	}
													if (lblFax.Text.Length > 0)
													{ %>
													<tr>
														<td align="right">Fax #</td>
														<td></td>
														<td><asp:label id="lblFax" runat="server"></asp:label></td>
													</tr>
													<%	}
													if (lblEmail.Text.Length > 0)
													{ %>
													<tr>
														<td align="right">Email</td>
														<td></td>
														<td><asp:label id="lblEmail" runat="server"></asp:label></td>
													</tr>
													<%	} %>
													<tr>
														<td align="right">Event Type</td>
														<td></td>
														<td><asp:label id="lblEventType" runat="server"></asp:label></td>
													</tr>
													<tr>
														<td align="right">Service Type</td>
														<td></td>
														<td><b><asp:label id="lblServType" runat="server" cssclass="JobItem"></asp:label></b></td>
													</tr>
													<tr>
														<td align="right">Location</td>
														<td></td>
														<td><asp:label id="lblLocation" runat="server"></asp:label></td>
													</tr>
													<%	if (lblDirections.Text.Length > 0)
													{ %>
													<tr>
														<td valign="top" align="right">Directions</td>
														<td></td>
														<td>
															<asp:label id="lblDirections" runat="server"></asp:label></td>
													</tr>
													<%	} %>
													<tr>
														<td align="right">Job Date</td>
														<td></td>
														<td><b><asp:label id="lblJobDate" runat="server" cssclass="JobItem"></asp:label></b></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="1" src="Images/Space.gif" alt="" class="CustTitle"></td>
														<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
														<td></td>
													</tr>
													<tr>
														<td align="right">Meal Time</td>
														<td></td>
														<td align="right"><asp:label id="lblMealTime" runat="server"></asp:label></td>
													</tr>
													<tr>
														<td align="right">Arrival Time</td>
														<td></td>
														<td align="right"><asp:label id="lblArrivalTime" runat="server"></asp:label></td>
													</tr>
													<tr>
														<td align="right">Depart Time</td>
														<td></td>
														<td align="right"><b><asp:label id="lblDepartureTime" runat="server" cssclass="JobItem"></asp:label></b></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="1" src="Images/Space.gif" alt="" class="CustTitle"></td>
														<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
														<td></td>
													</tr>
													<tr>
														<td align="right">Servings</td>
														<td></td>
														<td align="right">
															<b>
																<asp:label id="lblNumServing" runat="server" cssclass="JobItem"></asp:label></b>&nbsp; 
															(M
															<asp:label id="lblNumMenServing" runat="server"></asp:label>, W
															<asp:label id="lblNumWomenServing" runat="server"></asp:label>, C
															<asp:label id="lblNumChildServing" runat="server"></asp:label>)
														</td>
													</tr>
													<tr>
														<td align="right">Price</td>
														<td></td>
														<td><asp:label id="lblPricePerPerson" runat="server"></asp:label></td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
									<table cellspacing="0" cellpadding="0" border="0" width="100%">
										<tr>
											<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
										</tr>
										<tr>
											<td class="NoteBox">
												<asp:label id="lblJobNotes" runat="server"></asp:label>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<table cellspacing="0" cellpadding="0" border="0" align="center">
										<tr>
											<td></td>
											<td><img height="10" src="Images/Space.gif" alt="" width="20"></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="15"></td>
											<td></td>
										</tr>
										<tr>
											<td colspan="4">
												<hr>
											</td>
										</tr>
										<tr>
											<td class="JobSubTitle" align="center" colspan="4">FOOD</td>
										</tr>
										<% FoodCategory("Meats"); %>
										<% FoodCategory("Sides"); %>
										<% FoodCategory("Salads"); %>
										<% FoodCategory("Bread"); %>
										<% FoodCategory("Desserts"); %>
										<% FoodCategory("Drink"); %>
										<% FoodOther(); %>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td style="BORDER-LEFT: black 2px solid" width="1">&nbsp;</td>
					<td style="PADDING-LEFT: 5px; WIDTH: 400px" valign="top" align="center">
						<table cellspacing="0" cellpadding="0" border="0" height="100%" width="100%">
							<tr>
								<td valign="top">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="5"></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="5"></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="100"></td>
										</tr>
										<tr>
											<td class="JobSubTitle" align="center" colspan="6">SERVICES</td>
										</tr>
										<tr>
											<td align="right" colspan="3"><b><%= Globals.g.Company.Initials %>MCC</b></td>
											<td></td>
											<td align="left" colspan="2"><b>Customer</b></td>
										</tr>
										<% Services(); %>
									</table>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td></td>
											<td><img height="10" src="Images/Space.gif" alt="" width="5"></td>
											<td></td>
										</tr>
										<tr>
											<td colspan="3">
												<hr>
											</td>
										</tr>
										<tr>
											<td class="JobSubTitle" align="center" colspan="3">SUPPLIES</td>
										</tr>
										<tr>
											<td></td>
											<td></td>
											<td align="right"><b>Qty</b></td>
										</tr>
										<% Supplies(); %>
									</table>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td></td>
											<td><img width="5" height="10" src="Images/Space.gif" alt=""></td>
											<td></td>
											<td><img width="5" height="1" src="Images/Space.gif" alt=""></td>
											<td></td>
										</tr>
										<tr>
											<td colspan="5"><hr>
											</td>
										</tr>
										<tr>
											<td align="center" colspan="5" class="JobSubTitle">CREW</td>
										</tr>
										<tr>
											<td><b>Crew Member</b></td>
											<td></td>
											<td><b>Assignment</b></td>
											<td></td>
											<td align="right"><b>Time</b></td>
										</tr>
										<% Crew(); %>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>

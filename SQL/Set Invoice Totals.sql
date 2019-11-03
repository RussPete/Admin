-- create JobInvoicePrices records for jobs that don't have them, but need them
Insert Into JobInvoicePrices (JobRno, Seq, PriceType, InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, CreatedDtTm, CreatedUser)
Select JobRno, 1, 'Per Person', InvEventCount, 'servings', InvPricePerPerson, GetDate(), 'SysConv' 
From mcJobs Where JobRno Not In (Select JobRno From JobInvoicePrices) And (IsNull(InvEventCount, 0) <> 0 And IsNull(InvPricePerPerson, 0) <> 0)

-- calc extended prices
Update JobInvoicePrices Set 
InvPerPersonExtPrice	= InvPerPersonCount * InvPerPersonPrice,
InvPerItemExtPrice		= InvPerItemCount	* InvPerItemPrice,
InvAllInclExtPrice		= 1					* InvAllInclPrice

--select * from JobInvoicePrices

-- round the pmt 2 amount and required
Update mcJobs Set PmtAmt2 = Round(PmtAmt2, 2) Where PmtAmt2 Is Not Null
Update mcJobs Set CCReqAmt2 = Round(CCReqAmt2, 2) Where CCReqAmt2 <> Round(CCReqAmt2, 2)

-- update job SubTotAmt 
Update mcJobs Set
SubTotAmt = IsNull((Select Sum(
	Case PriceType 
		When 'Per Person'		Then InvPerPersonExtPrice
		When 'Per Item'			Then InvPerItemExtPrice
		When 'All Inclusive'	Then InvAllInclExtPrice
		Else						 0
	End 
) From JobInvoicePrices Where JobRno = j.JobRno), 0)
From mcJobs j

-- set pre tax sub total & sales tax total
Update mcJobs Set
PreTaxSubTotAmt = 
	IsNull(SubTotAmt, 0) + 
	IsNull(ServiceAmt, 0) + 
	IsNull(DeliveryAmt, 0) + 
	IsNull(ChinaAmt, 0) + 
	IsNull(AddServiceAmt, 0) + 
	IsNull(FuelTravelAmt, 0) + 
	IsNull(FacilityAmt, 0) + 
	IsNull(GratuityAmt, 0) + 
	IsNull(RentalsAmt, 0) + 
	IsNull(Adj1Amt, 0) + 
	IsNull(Adj2Amt, 0),
SalesTaxTotAmt = Round(
	Case When IsNull(TaxExemptFlg, 0) = 0 
	Then
 		IsNull(SubTotAmt, 0)		* IsNull(SubTotTaxPct, 0)		/ 100 + 
		IsNull(ServiceAmt, 0)		* IsNull(ServiceTaxPct, 0)		/ 100 + 
		IsNull(DeliveryAmt, 0)		* IsNull(DeliveryTaxPct, 0)		/ 100 + 
		IsNull(ChinaAmt, 0)			* IsNull(ChinaTaxPct, 0)		/ 100 + 
		IsNull(AddServiceAmt, 0)	* IsNull(AddServiceTaxPct, 0)	/ 100 + 
		IsNull(FuelTravelAmt, 0)	* IsNull(FuelTravelTaxPct, 0)	/ 100 + 
		IsNull(FacilityAmt, 0)		* IsNull(FacilityTaxPct, 0)		/ 100 + 
		IsNull(GratuityAmt, 0)		* IsNull(GratuityTaxPct, 0)		/ 100 + 
		IsNull(RentalsAmt, 0)		* IsNull(RentalsTaxPct, 0)		/ 100 + 
		IsNull(Adj1Amt, 0)			* IsNull(Adj1TaxPct, 0)			/ 100 + 
		IsNull(Adj2Amt, 0)			* IsNull(Adj2TaxPct, 0)			/ 100
	Else 0 
	End, 
2)

-- set invoice total
Update mcJobs Set
InvTotAmt = PreTaxSubTotAmt + SalesTaxTotAmt,
InvBalAmt = (PreTaxSubTotAmt + SalesTaxTotAmt) - Round(IsNull(PmtAmt1, 0) + IsNull(PmtAmt2, 0) + IsNull(PmtAmt3, 0), 2)

-- credit card fee amount
Update mcJobs Set
CCFeeAmt = 	
	Case When IsNull(CCPmtFeeFlg, 0) = 1 
	Then 
		Round((Case When InvBalAmt > 0 Then InvBalAmt Else 0 End) * IsNull(CCFeePct, 0) / 100, 2) + 
		Case When PmtMethod1 = 'CreditCard' Then Round(CCReqAmt1  * IsNull(CCFeePct, 0) / 100, 2) Else 0 End +
		Case When PmtMethod2 = 'CreditCard' Then Round(CCReqAmt2  * IsNull(CCFeePct, 0) / 100, 2) Else 0 End +
		Case When PmtMethod3 = 'CreditCard' Then Round(CCReqAmt3  * IsNull(CCFeePct, 0) / 100, 2) Else 0 End 
	Else 0 
	End

-- adjust totals for credit card fee
;With CCFeeSTax as
(
	select JobRno,
	Round(Case When IsNull(TaxExemptFlg, 0) = 0 Then 
		(IsNull(CCFeeAmt, 0) - 
			Case When PmtMethod1 = 'CreditCard' Then Round(CCReqAmt1 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End -
			Case When PmtMethod2 = 'CreditCard' Then Round(CCReqAmt2 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End -
			Case When PmtMethod3 = 'CreditCard' Then Round(CCReqAmt3 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End
		) * IsNull(CCFeeTaxPct, 0) / 100 Else 0 End, 2) +
	Case When IsNull(TaxExemptFlg, 0) = 0 Then Round((Case When PmtMethod1 = 'CreditCard' Then Round(CCReqAmt1 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End) * IsNull(CCFeeTaxPct, 0) / 100, 2) Else 0 End +
	Case When IsNull(TaxExemptFlg, 0) = 0 Then Round((Case When PmtMethod2 = 'CreditCard' Then Round(CCReqAmt2 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End) * IsNull(CCFeeTaxPct, 0) / 100, 2) Else 0 End +
	Case When IsNull(TaxExemptFlg, 0) = 0 Then Round((Case When PmtMethod3 = 'CreditCard' Then Round(CCReqAmt3 * IsNull(CCFeePct, 0) / 100, 2) Else 0 End) * IsNull(CCFeeTaxPct, 0) / 100, 2) Else 0 End
	as CCFeeTax
	from mcJobs
	Where CCFeeAmt <> 0
)
Update mcJobs Set
PreTaxSubTotAmt = PreTaxSubTotAmt + IsNull(CCFeeAmt, 0),
SalesTaxTotAmt	= SalesTaxTotAmt                        + CCFeeTax,
InvTotAmt		= InvTotAmt       + IsNull(CCFeeAmt, 0) + CCFeeTax,
InvBalAmt		= InvBalAmt       + IsNull(CCFeeAmt, 0) + CCFeeTax
From CCFeeSTax t Inner Join mcJobs j on t.JobRno = j.JobRno

--select JobRno, SubTotAmt, CCPmtFeeFlg, CCFeeAmt, PreTaxSubTotAmt, TaxExemptFlg, SalesTaxTotAmt, InvTotAmt, IsNull(PmtAmt1, 0) + IsNull(PmtAmt2, 0) + IsNull(PmtAmt3, 0) As Payments, InvBalAmt from mcJobs where InvBalAmt <> 0 --where JobRno = 7119
select JobRno, SubTotAmt, CCPmtFeeFlg, CCFeeAmt, PreTaxSubTotAmt, TaxExemptFlg, SalesTaxTotAmt, InvTotAmt, PmtAmt1, PmtAmt2, PmtAmt3, InvBalAmt from mcJobs where InvBalAmt <> 0 --where JobRno = 7119


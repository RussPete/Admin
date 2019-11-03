truncate table Payments
DBCC CHECKIDENT (Payments, RESEED, 1)
;
With Pmts as
(
	select JobRno, 1 As Seq, PmtAmt1, PmtDt1, PmtRef1, PmtMethod1, PmtType1, CCReqAmt1, CCType1, CCNum1, CCExpDt1, CCSecCode1, CCName1, CCAddr1, CCZip1, CCStatus1, CCResult1, CCAuth1, CCRef1, CCPmtDtTm1, CCPmtUser1, CCPrintDtTm1, CCPrintUser1, GetDate() As CreatedDtTm, 'SysConv' As CreatedUser, GetDate() As UpdatedDtTm, 'SysConv' As UpdatedUser from mcJobs union
	select JobRno, 2 As Seq, PmtAmt2, PmtDt2, PmtRef2, PmtMethod2, PmtType2, CCReqAmt2, CCType2, CCNum2, CCExpDt2, CCSecCode2, CCName2, CCAddr2, CCZip2, CCStatus2, CCResult2, CCAuth2, CCRef2, CCPmtDtTm2, CCPmtUser2, CCPrintDtTm2, CCPrintUser2, GetDate() As CreatedDtTm, 'SysConv' As CreatedUser, GetDate() As UpdatedDtTm, 'SysConv' As UpdatedUser from mcJobs union
	select JobRno, 3 As Seq, PmtAmt3, PmtDt3, PmtRef3, PmtMethod3, PmtType3, CCReqAmt3, CCType3, CCNum3, CCExpDt3, CCSecCode3, CCName3, CCAddr3, CCZip3, CCStatus3, CCResult3, CCAuth3, CCRef3, CCPmtDtTm3, CCPmtUser3, CCPrintDtTm3, CCPrintUser3, GetDate() As CreatedDtTm, 'SysConv' As CreatedUser, GetDate() As UpdatedDtTm, 'SysConv' As UpdatedUser from mcJobs 
)
insert into Payments (JobRno, Seq, Amount, PaymentDt, Reference, Method, Type, CCReqAmt, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCAddr, CCZip, CCStatus, CCResult, CCAuth, CCReference, CCPmtDtTm, CCPmtUser, CCPrintDtTm, CCPrintUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
select * from Pmts
where isnull(PmtMethod1, '') <> '' or isnull(PmtType1, '') <> ''
Order by JobRno, Seq

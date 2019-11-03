-- convert Jobs data into Contacts and Customers

delete from customers
delete from contacts

declare 
	@JobRno int,
	@Customer varchar(50), 
	@JobType varchar(20), 
	@EventType varchar(50), 
	@TaxExemptFlg tinyint,
	@ContactName varchar(50), 
	@ContactPhone varchar(20), 
	@ContactCell varchar(20), 
	@ContactFax varchar(20), 
	@ContactEmail varchar(80),
	@CustomerRno int,
	@ContactRno int

declare curJobs cursor for 
	select JobRno, Customer, JobType, EventType, TaxExemptFlg, ContactName, ContactPhone, ContactCell, IsNull(ContactFax, '') as ContactFax, ContactEmail 
	from mcJobs 
	order by JobRno

open curJobs
fetch next from curJobs into 
	@JobRno, @Customer, @JobType, @EventType, @TaxExemptFlg, @ContactName, @ContactPhone, @ContactCell, @ContactFax, @ContactEmail
while @@FETCH_STATUS = 0
begin

	if Len(@ContactName) = 0 
	begin
		set @ContactName = @Customer
		set @Customer = ''
	end

	if Len(@Customer) = 0
	begin
		set @CustomerRno = null
	end
	else
	begin
	-- Customers Name, TaxExemptFlg, CompanyFlg
		select @CustomerRno = (select CustomerRno from Customers Where Name = @Customer)
		if @CustomerRno is null
		begin
			insert into Customers (Name, TaxExemptFlg, CompanyFlg, CreatedDtTm, CreatedUser) Values (@Customer, @TaxExemptFlg, case when @JobType = 'Corporate' then 1 else 0 end, GetDate(), 'Conversion')
			select @CustomerRno = Scope_Identity()
		end
	end

	-- Contacts Name, Phone, Cell, Fax, Email, Title, ExportDtTm, ExportId
	select @ContactRno = (select ContactRno from Contacts 
		where Name = @ContactName and 
			IsNull(Phone, '') = @ContactPhone and 
			IsNull(Cell, '') = @ContactCell and 
			IsNull(Fax, '') = @ContactFax and 
			IsNull(Email, '') = @ContactEmail and 
			(CustomerRno = @CustomerRno or CustomerRno is null and @CustomerRno is null))
	if @ContactRno is null
	begin
		insert into Contacts (CustomerRno, Name, Phone, Cell, Fax, Email, CreatedDtTm, CreatedUser) Values (@CustomerRno, @ContactName, @ContactPhone, @ContactCell, @ContactFax, @ContactEmail, GetDate(), 'Conversion')
		select @ContactRno = Scope_Identity()
	end

	-- Jobs JobDesc, ContactRno
	Update mcJobs set JobDesc = Left(
		case when Len(@Customer) > 0 and Len(@EventType) > 0 then   
			@Customer + ' - ' + @EventType else 
		case when Len(@Customer) > 0 and Len(@ContactName) > 0 then 
			@Customer + ' - ' + @ContactName else 
		case when Len(@Customer) = 0 and Len(@ContactName) > 0 and Len(@EventType) > 0 then 
			@ContactName + ' - ' + @EventType else 
		case when Len(@Customer) = 0 then 
			@ContactName else 
			@Customer 
		end 
		end 
		end 
		end, 80), 
		ContactRno = @ContactRno
		where JobRno = @JobRno

	fetch next from curJobs into 
		@JobRno, @Customer, @JobType, @EventType, @TaxExemptFlg, @ContactName, @ContactPhone, @ContactCell, @ContactFax, @ContactEmail
end
close curJobs
deallocate curJobs
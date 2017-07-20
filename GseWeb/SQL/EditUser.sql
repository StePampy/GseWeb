UPDATE users
	SET  Password   =   @Password
		,FirstName	=   @FirstName
		,LastName	=   @LastName
		,Email		=   @Email
		,BirthDate	=   @BirthDate
		,Phone		=   @Phone
		,RoleId		=   @RoleId
		,Hours_ProfileId = @ProfileId
WHERE UserId = @UserId
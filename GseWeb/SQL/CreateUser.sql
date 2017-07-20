INSERT INTO users 
SELECT @UserId
		,@Password
        ,@FirstName
        ,@LastName
        ,@Email
        ,@BirthDate
        ,@Phone
        ,@RoleId
		,@ProfileId
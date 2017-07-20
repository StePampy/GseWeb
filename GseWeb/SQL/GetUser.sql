SELECT UserId,
		Password,
        FirstName,
        LastName,
        Email,
        BirthDate,
        Phone,
		RoleId,
        userroles.Role,
		Hours_ProfileId
FROM users
inner join userroles
on users.roleid = userroles.id
where UserId = @UserId
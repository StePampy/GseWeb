SELECT UserId
        ,Date
        ,Start
        ,End
        ,Break
        ,Travel
        ,WorkTime
        ,DiffTime
		,Note
		,OffSite
        ,WorkTypeId
FROM v_work_calculate
where UserId = @UserId
and Date = @Date 
select UserId
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
from v_work_calculate
where UserId = @UserId 
and year(date) = @Year
and MONTH(Date) = @Month
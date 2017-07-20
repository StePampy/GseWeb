DROP TABLE IF EXISTS tmpTOT;
CREATE TEMPORARY TABLE tmpTOT (
	SELECT sec_to_time(SUM(time_to_sec(WorkTime))) as Value
	from v_work_calculate
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	group by UserId
);


DROP TABLE IF EXISTS tmpTRAV;
CREATE TEMPORARY TABLE tmpTRAV (
	SELECT sec_to_time(SUM(time_to_sec(Travel))) as Value
	from v_work_calculate
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	group by UserId
);

DROP TABLE IF EXISTS tmpGRP;
CREATE TEMPORARY TABLE tmpGRP (
	select  UserId
			,WorkTypeId
            ,work_type.Description
			,sec_to_time(abs(SUM(time_to_sec(DiffTime)))) as Value
	from v_work_calculate
    INNER JOIN work_type
    ON v_work_calculate.WorkTypeId = work_type.ID
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	AND WorkTypeId > 1
	group by UserId, WorkTypeId
);

DROP TABLE IF EXISTS tmpORD;
CREATE TEMPORARY TABLE tmpORD (
	select timediff((select Value from tmpTOT), CAST(IF(ISNULL(Value), '00:00:00', Value) AS TIME)) as Value
    from
	(select 2 as Ref)tref
	left join tmpGRP
	on tref.Ref = tmpGRP.WorkTypeId
);

SELECT UserId
		,WorkTypeId
        ,Description
        ,CAST(IF(ISNULL(Value), '00:00:00', Value) AS TIME) AS Value 
from (
	select @UserId as UserId, 1 as WorkTypeId, 'Ordinario' as Description, (select Value from tmpORD) as Value
	union
	select UserId, WorkTypeId, Description, Value from tmpGRP
	union
	select @UserId, 999, 'Totale Viaggio', (select Value from tmpTRAV)
	union
	select @UserId, 999, 'Totale Lavorativo', (select Value from tmpTOT) 
)T1


/*
DROP TABLE IF EXISTS tmpTOT;
CREATE TEMPORARY TABLE tmpTOT (
	SELECT sec_to_time(SUM(time_to_sec(WorkTime))) as Value
	from v_work_calculate
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	group by UserId
);

DROP TABLE IF EXISTS tmpTRAV;
CREATE TEMPORARY TABLE tmpTRAV (
	SELECT sec_to_time(SUM(time_to_sec(Travel))) as Value
	from v_work_calculate
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	group by UserId
);

DROP TABLE IF EXISTS tmpGRP;
CREATE TEMPORARY TABLE tmpGRP (
	select  UserId
			,WorkTypeId
            ,work_type.Description
			,sec_to_time(abs(SUM(time_to_sec(DiffTime)))) as Value
	from v_work_calculate
    INNER JOIN work_type
    ON v_work_calculate.WorkTypeId = work_type.ID
	where UserId = @UserId 
	and year(date) = @Year
	and MONTH(Date) = @Month
	AND WorkTypeId > 1
	group by UserId, WorkTypeId
);

DROP TABLE IF EXISTS tmpORD;
CREATE TEMPORARY TABLE tmpORD (
	select TIMEDIFF((select Value from tmpTOT), (select Value from tmpGRP where WorkTypeId = 2)) as Value
);

SELECT UserId
		,WorkTypeId
        ,Description
        ,CAST(IF(ISNULL(Value), '00:00:00', Value) AS TIME) AS Value 
from (
	select @UserId as UserId, 1 as WorkTypeId, 'Ordinario' as Description, (select Value from tmpORD) as Value
	union
	select UserId, WorkTypeId, Description, Value from tmpGRP
	union
	select @UserId, 999, 'Totale Viaggio', (select Value from tmpTRAV)
	union
	select @UserId, 999, 'Totale Lavorativo', (select Value from tmpTOT) 
)T1
*/
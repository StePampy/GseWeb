DROP TABLE IF EXISTS tmpDays;
CREATE TEMPORARY TABLE tmpDays (
SELECT Date
FROM
(
    SELECT
        MAKEDATE(YEAR(@Date),1) +
        INTERVAL (MONTH(@Date)-1) MONTH +
        INTERVAL daynum DAY Date
    FROM
    (
        SELECT t*10+u daynum
        FROM
            (SELECT 0 t UNION SELECT 1 UNION SELECT 2 UNION SELECT 3) A,
            (SELECT 0 u UNION SELECT 1 UNION SELECT 2 UNION SELECT 3
            UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7
            UNION SELECT 8 UNION SELECT 9) B
        ORDER BY daynum
    ) AA
) AAA
WHERE MONTH(Date) = MONTH(@Date)
order by 1
);

select tmpDays.Date
		,CAST(IF(ISNULL(Start), '00:00:00', Start) AS TIME) AS Start
        ,CAST(IF(ISNULL(End), '00:00:00', End) AS TIME) AS End
        ,CAST(IF(ISNULL(Break), '00:00:00', Break) AS TIME) AS Break
        ,CAST(IF(ISNULL(Travel), '00:00:00', Travel) AS TIME) AS Travel
        ,IF(ISNULL(WorkTypeId), 0, WorkTypeId) as WorkTypeId
        ,IF(ISNULL(Note), '', Note) as Note
        ,IF(ISNULL(OffSite), 0, OffSite) as OffSite
        ,CAST(IF(ISNULL(WorkTime), '00:00:00', WorkTime) AS TIME) AS WorkTime
        ,CAST(IF(ISNULL(DiffTime), '00:00:00', DiffTime) AS TIME) AS DiffTime
from tmpDays
left join(select * from v_work_calculate where UserId = @UserId)t2
on tmpDays.Date = t2.Date
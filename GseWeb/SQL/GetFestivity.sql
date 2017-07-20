SELECT date
		,description
FROM festivity
where year(date) = @Year
and MONTH(Date) = @Month
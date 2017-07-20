insert into work_hours
select @UserId
		,@Date
        ,@Start
        ,@End
        ,@Break
        ,@Travel
        ,@WorkTypeId
		,@Note
		,@OffSite
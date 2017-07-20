UPDATE work_hours
	SET  Start   =   @Start
		,End	=   @End
		,Break	=   @Break
		,Travel		=   @Travel
		,WorkTypeId	=   @WorkTypeId
		,Note = @Note
		,OffSite = @OffSite
WHERE UserId = @UserId 
and Date = @Date
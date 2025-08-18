-- =============================================
-- Author:		Scott Cook
-- Create date: 11/10/14
-- Description:	Parses XML string and writes the data to the temporary table.
-- =============================================
CREATE PROCEDURE [dbo].[Parse_XML](
                                   @XMLString nvarchar(MAX),
                                   @strtX BIGINT,
					 			   @endX BIGINT OUTPUT,
								   @DataStr nvarchar(MAX) OUTPUT
								  )
AS
BEGIN
	DECLARE @StartChar CHAR(1)
	DECLARE @endTmp BIGINT

	SET @strtX = CHARINDEX('<', @XMLString, @strtX)
	SET @StartChar = SUBSTRING(@XMLString, @strtX + 1, 1)

	IF @StartChar = '/'
	  SET @endX = CHARINDEX('>', @XMLString, @strtX)
	ELSE
	  BEGIN  
        SET @endTmp = CHARINDEX('>', @XMLString, @strtX)
        IF SUBSTRING(@XMLString, @endTmp - 1, 1) = '/'
          SET @endX = @endTmp
	    ELSE
	      BEGIN
		    SET @endTmp = CHARINDEX('<', @XMLString, @endTmp)
		    IF SUBSTRING(@XMLString, @endTmp + 1, 1) = '/'
              SET @endX = CHARINDEX('>', @XMLString, @endTmp + 1)
		    ELSE
		      SET @endX = @endTmp - 1
	      END
	  END

	SET @DataStr = SUBSTRING(@XMLString, @strtX, (@endX - @strtX) + 1)
	SET @endX = @endX + 1

--select @DataStr
--select @endX

END

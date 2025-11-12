CREATE OR ALTER PROCEDURE dbo.SP_TASK
  @TipoOpcion   INT,                    
  @Page         INT           = 1,
  @Size         INT           = 10,
  @SortField    NVARCHAR(50)  = N'createdAt',
  @SortOrder    INT           = -1,          
  @UserName     NVARCHAR(100) = NULL,
  @Email        NVARCHAR(100) = NULL,
  @Title        NVARCHAR(200) = NULL,
  @GlobalUser   NVARCHAR(100) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  IF (@Page < 1) SET @Page = 1;
  IF (@Size < 1 OR @Size > 200) SET @Size = 10;

  IF @TipoOpcion = 1
  BEGIN
    DECLARE @sql NVARCHAR(MAX) =
    N'
    WITH Q AS (
      SELECT
        t.id, t.title, t.description, t.createdAt, t.updatedAt,
        u.id AS userId, u.fullName, u.email,
        s.id AS statusId, s.name AS statusName
      FROM TBL_TASK t
      JOIN TBL_USER u ON u.id = t.idUser
      JOIN TBL_STATUSTASK s ON s.id = t.idStatus
      WHERE ( @UserName   IS NULL OR u.fullName LIKE ''%'' + @UserName + ''%'' )
        AND ( @Email      IS NULL OR u.email    LIKE ''%'' + @Email    + ''%'' )
        AND ( @Title      IS NULL OR t.title    LIKE ''%'' + @Title    + ''%'' )
        AND ( @GlobalUser IS NULL OR u.fullName LIKE ''%'' + @GlobalUser + ''%'' OR u.email LIKE ''%'' + @GlobalUser + ''%'' )
    )
    SELECT *,
           COUNT(*) OVER() AS TotalCount
    FROM Q
    ORDER BY ';

    DECLARE @sf NVARCHAR(50) = LOWER(@SortField);
    IF @sf NOT IN (N'title', N'createdat', N'updatedat', N'user.fullname', N'user.email', N'status.name')
       SET @sf = N'createdat';

    SET @sql +=
      CASE @sf
        WHEN N'title'          THEN N'title'
        WHEN N'updatedat'      THEN N'updatedAt'
        WHEN N'user.fullname'  THEN N'fullName'
        WHEN N'user.email'     THEN N'email'
        WHEN N'status.name'    THEN N'statusName'
        ELSE N'createdAt'
      END + CASE WHEN @SortOrder = 1 THEN N' ASC' ELSE N' DESC' END + N'
    OFFSET (@Page - 1) * @Size ROWS FETCH NEXT @Size ROWS ONLY;';

    EXEC sp_executesql
      @sql,
      N'@Page int, @Size int, @UserName nvarchar(100), @Email nvarchar(100), @Title nvarchar(200), @GlobalUser nvarchar(100)',
      @Page=@Page, @Size=@Size, @UserName=@UserName, @Email=@Email, @Title=@Title, @GlobalUser=@GlobalUser;
    RETURN;
  END;

  IF @TipoOpcion = 2
  BEGIN
    -- Métricas; reutilizamos mismos filtros
    SELECT
      Total        = COUNT(*),
      Completadas  = SUM(CASE WHEN s.name = 'Listo' THEN 1 ELSE 0 END),
      Pendientes   = SUM(CASE WHEN s.name <> 'Listo' THEN 1 ELSE 0 END),
      Bloqueadas   = SUM(CASE WHEN s.name LIKE 'Bloqueado%' THEN 1 ELSE 0 END),
      PorHacer     = SUM(CASE WHEN s.name LIKE 'Por Hacer%' THEN 1 ELSE 0 END),
      EnCurso      = SUM(CASE WHEN s.name LIKE 'En Curso%' THEN 1 ELSE 0 END),
      QA           = SUM(CASE WHEN s.name LIKE 'QA%' THEN 1 ELSE 0 END)
    FROM TBL_TASK t
    JOIN TBL_USER u ON u.id = t.idUser
    JOIN TBL_STATUSTASK s ON s.id = t.idStatus
    WHERE ( @UserName   IS NULL OR u.fullName LIKE '%' + @UserName + '%' )
      AND ( @Email      IS NULL OR u.email    LIKE '%' + @Email    + '%' )
      AND ( @Title      IS NULL OR t.title    LIKE '%' + @Title    + '%' )
      AND ( @GlobalUser IS NULL OR u.fullName LIKE '%' + @GlobalUser + '%' OR u.email LIKE '%' + @GlobalUser + '%' );
    RETURN;
  END;

  RAISERROR('TipoOpcion no soportado. Use 1 (Paginado) o 2 (Métricas).', 16, 1);
END
GO
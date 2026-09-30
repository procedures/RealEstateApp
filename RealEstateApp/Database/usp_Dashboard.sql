/* =====================================================================
   DASHBOARD-ИЙН СТАТИСТИК ПРОЦЕДУРУУД
   Баазан дээрээ нэг удаа ажиллуулна (CREATE OR ALTER тул дахин ажиллуулж болно).
   ===================================================================== */
USE [RealEstateApp];
GO

/* ---------------------------------------------------------------------
   1) АДМИН: агентуудын чансаа
      - Хамгийн олон хөрөнгө бүртгэсэн
      - Хамгийн олон (зөвшөөрөгдсөн) хаалт хийсэн
      Нэг л жагсаалт буцаана. UI талд PropertyCount эсвэл ClosureCount-оор
      эрэмбэлж хоёр самбар харуулна.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_Dashboard_AdminStats
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Top < 1 SET @Top = 10;

    /* 2026-09: Хамтран борлуулсан (CoAgentPhone) хаалтыг ХОЁР агентад
       нь БҮТЭН дүнгээрээ тооцдог боллоо — /admin/stats/sales болон
       /agent/stats-тэй ижил логик (dbo.Agents.Phone-оор тааруулна). */
    ;WITH PropAgg AS (
        SELECT AgentId, COUNT(*) AS PropertyCount
        FROM   dbo.Properties
        GROUP  BY AgentId
    )
    SELECT TOP (@Top)
           a.AgentId,
           a.LastName,
           a.FirstName,
           a.PhotoUrl,
           ISNULL(p.PropertyCount, 0)  AS PropertyCount,
           (SELECT COUNT(DISTINCT pc.ClosureId)
              FROM   dbo.PropertyClosures AS pc
              WHERE  pc.ApprovalStatus = 2          -- зөвшөөрөгдсөн хаалт
                     AND (pc.SubmittedByAgentId = a.AgentId
                          OR pc.CoAgentPhone = a.Phone))       AS ClosureCount,
           (SELECT ISNULL(SUM(pc.SoldTotalPrice), 0)
              FROM   dbo.PropertyClosures AS pc
              WHERE  pc.ApprovalStatus = 2
                     AND (pc.SubmittedByAgentId = a.AgentId
                          OR pc.CoAgentPhone = a.Phone))       AS SoldTotalAmount
    FROM   dbo.Agents AS a
           LEFT JOIN PropAgg  AS p ON p.AgentId = a.AgentId
    WHERE  a.IsActive = 1
           /* Зөвхөн агентыг харуулах бол доорхыг идэвхжүүлнэ:
           AND a.Role = N'Agent' */
    ORDER BY PropertyCount DESC, ClosureCount DESC;
END
GO


/* ---------------------------------------------------------------------
   2) АГЕНТ: өөрийн хувийн статистик (нэг мөр)
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_Dashboard_AgentStats
    @AgentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        /* Хөрөнгийн тоо, төлөвөөр */
        (SELECT COUNT(*) FROM dbo.Properties
          WHERE AgentId = @AgentId)                       AS TotalProperties,
        (SELECT COUNT(*) FROM dbo.Properties
          WHERE AgentId = @AgentId AND Status = 2)         AS PendingCount,      -- хүлээгдэж буй
        (SELECT COUNT(*) FROM dbo.Properties
          WHERE AgentId = @AgentId AND Status = 3)         AS PublishedCount,    -- нийтлэгдсэн
        (SELECT COUNT(*) FROM dbo.Properties
          WHERE AgentId = @AgentId AND Status = 4)         AS ClosureRequestCount, -- хаалт хүсэлт
        (SELECT COUNT(*) FROM dbo.Properties
          WHERE AgentId = @AgentId AND Status = 5)         AS ClosedCount,       -- хаагдсан

        /* Зөвшөөрөгдсөн хаалт ба борлуулалтын нийт дүн */
        (SELECT COUNT(*) FROM dbo.PropertyClosures
          WHERE SubmittedByAgentId = @AgentId AND ApprovalStatus = 2)
                                                           AS ApprovedClosures,
        (SELECT ISNULL(SUM(SoldTotalPrice), 0) FROM dbo.PropertyClosures
          WHERE SubmittedByAgentId = @AgentId AND ApprovalStatus = 2)
                                                           AS SoldTotalAmount,

        /* Үзлэгийн хүсэлт */
        (SELECT COUNT(*) FROM dbo.PropertyViewings
          WHERE AgentId = @AgentId)                        AS TotalViewings,
        (SELECT COUNT(*) FROM dbo.PropertyViewings
          WHERE AgentId = @AgentId AND Status = 1)         AS NewViewings;       -- шинэ
END
GO

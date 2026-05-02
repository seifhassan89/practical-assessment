-- to sql server to rollback the transaction automatically if an error occurs
SET XACT_ABORT ON;

-- to get the current date and time
DECLARE @ProcessingDate DATETIME = GETDATE();

BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. Update existing snapshot records
    UPDATE target
    SET
        target.PurchaseStatus = source.PurchaseStatus,
        target.ProcessingDate = @ProcessingDate
    FROM dbo.PurchasesSnapshot AS target
    INNER JOIN dbo.Purchases AS source
        ON  target.WebsiteId      = source.WebsiteId
        AND target.ProductId      = source.ProductId
        AND target.UnitOfMeasureId = source.UnitOfMeasureId;

    -- 2. Insert missing snapshot records
    INSERT INTO dbo.PurchasesSnapshot
    (
        WebsiteId,
        ProductId,
        UnitOfMeasureId,
        PurchaseStatus,
        ProcessingDate
    )
    SELECT
        source.WebsiteId,
        source.ProductId,
        source.UnitOfMeasureId,
        source.PurchaseStatus,
        @ProcessingDate
    FROM dbo.Purchases AS source
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.PurchasesSnapshot AS target WITH (UPDLOCK, HOLDLOCK)
        WHERE target.WebsiteId       = source.WebsiteId
          AND target.ProductId        = source.ProductId
          AND target.UnitOfMeasureId  = source.UnitOfMeasureId
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
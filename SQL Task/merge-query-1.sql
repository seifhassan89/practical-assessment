MERGE dbo.PurchasesSnapshot AS target
USING dbo.Purchases AS source
ON  target.WebsiteId = source.WebsiteId
AND target.ProductId = source.ProductId
AND target.UnitOfMeasureId = source.UnitOfMeasureId

WHEN MATCHED THEN
    UPDATE SET
        target.PurchaseStatus = source.PurchaseStatus,
        target.ProcessingDate = GETDATE()

WHEN NOT MATCHED BY TARGET THEN
    INSERT
    (
        WebsiteId,
        ProductId,
        UnitOfMeasureId,
        PurchaseStatus,
        ProcessingDate
    )
    VALUES
    (
        source.WebsiteId,
        source.ProductId,
        source.UnitOfMeasureId,
        source.PurchaseStatus,
        GETDATE()
    );
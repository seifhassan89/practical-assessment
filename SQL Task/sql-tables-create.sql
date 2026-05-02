CREATE TABLE [dbo].[Purchases](
[WebsiteId] [nvarchar](50) NOT NULL,
[ProductId] [nvarchar](50) NOT NULL,
[UnitOfMeasureId] [nvarchar](50) NOT NULL,
[PurchaseStatus] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED
(
[WebsiteId] ASC,
[ProductId] ASC,
[UnitOfMeasureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON
[PRIMARY]
) ON [PRIMARY]
GO
-----------------
CREATE TABLE [dbo].[PurchasesSnapshot](
[WebsiteId] [nvarchar](50) NOT NULL,
[ProductId] [nvarchar](50) NOT NULL,
[UnitOfMeasureId] [nvarchar](50) NOT NULL,
[PurchaseStatus] [nvarchar](50) NOT NULL,
[ProcessingDate] [datetime] NOT NULL,
CONSTRAINT [PK_PurchasesSnapshot] PRIMARY KEY CLUSTERED
(
[WebsiteId] ASC,
[ProductId] ASC,
[UnitOfMeasureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON
[PRIMARY]
) ON [PRIMARY]
GO
-----------------
INSERT [dbo].[Purchases] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus]) VALUES (N'Store1', N'Apples', N'Box', N'InProgress')
GO
INSERT [dbo].[Purchases] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus]) VALUES (N'Store1', N'Apples', N'Piece', N'Paid')
GO
INSERT [dbo].[Purchases] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus]) VALUES (N'Store1', N'Bicycle', N'Piece', N'Paid')
GO
INSERT [dbo].[Purchases] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus]) VALUES (N'Store2', N'Apples', N'Piece', N'InProgress')
GO
5
INSERT [dbo].[Purchases] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus]) VALUES (N'Store2', N'Carrots', N'Box', N'Paid')
GO
INSERT [dbo].[PurchasesSnapshot] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus], [ProcessingDate]) VALUES (N'Store1', N'Apples', N'Box',
N'InProgress', CAST(N'2025-04-15T11:40:18.327' AS DateTime))
GO
INSERT [dbo].[PurchasesSnapshot] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus], [ProcessingDate]) VALUES (N'Store1', N'Bicycle', N'Piece', N'Paid',
CAST(N'2025-04-15T11:40:18.327' AS DateTime))
GO
INSERT [dbo].[PurchasesSnapshot] ([WebsiteId], [ProductId], [UnitOfMeasureId],
[PurchaseStatus], [ProcessingDate]) VALUES (N'Store2', N'Apples', N'Piece',
N'InProgress', CAST(N'2025-04-15T11:40:18.327' AS DateTime))
GO
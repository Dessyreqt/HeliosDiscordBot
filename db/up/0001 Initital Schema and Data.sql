USE [HeliosDiscordBot]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 4/5/2021 12:02:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notification](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[ChannelId] [varchar](30) NOT NULL,
	[Latitude] [decimal](10, 7) NOT NULL,
	[Longitude] [decimal](10, 7) NOT NULL,
	[NotifySunrise] [int] NULL,
	[NotifySunset] [int] NULL,
	[NextNotifySunriseUtc] [datetime] NULL,
	[NextNotifySunsetUtc] [datetime] NULL,
	[Timezone] [varchar](100) NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

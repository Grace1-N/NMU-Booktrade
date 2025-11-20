-- Script to remove ratings/reviews functionality from NMU BookTrade database
-- Execute this script to clean up the database after removing ratings feature

-- Drop foreign key constraints that reference the Review table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Review_Sale')
    ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_Sale];

-- Drop stored procedures related to reviews
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAverageReviewRatingBySeller]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[GetAverageReviewRatingBySeller];

-- Drop the Review table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Review]') AND type in (N'U'))
    DROP TABLE [dbo].[Review];

-- Optional: Add a comment to document the change
PRINT 'Ratings and reviews functionality has been removed from the database.';
PRINT 'The Review table and related stored procedures have been dropped.';

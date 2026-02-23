db = db.getSiblingDB('activityDB'); 

// Drop the existing collection if it exists (optional, for testing)
db.activities.drop();

// Create collection with case-insensitive collation
db.createCollection("activities", {
    collation: { locale: "en", strength: 1 }
});

// Create indexes to optimize queries
db.activities.createIndex({ ActivityType: 1 });
db.activities.createIndex({ ActivityAt: -1 });
db.activities.createIndex({ UserSession_Id: 1 });
db.activities.createIndex({ Tenant_Id: 1 });
db.activities.createIndex({ Entity_Id: 1 });
db.activities.createIndex({ EntityTypeName: 1 });

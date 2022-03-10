db = db.getSiblingDB('tripbooker-db');

db.createCollection('sample-collection');

db.createUser({
    user: 'dev1',
    pwd: 'password',
    roles: [
      {
        role: 'readWrite',
        db: 'tripbooker-db',
      },
    ]
  });
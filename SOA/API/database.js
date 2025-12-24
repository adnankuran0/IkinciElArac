const mysql = require('mysql2/promise');

// Veritabanı bağlantı havuzu (Connection Pool)
// appsettings.json'daki bağlantı bilgileriyle aynı
const pool = mysql.createPool({
    host: 'localhost',
    port: 3306,
    user: 'root',
    password: '8002',
    database: 'arackiralama',
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
});

// Test bağlantısı
pool.getConnection()
    .then(connection => {
        console.log('✅ MySQL veritabanına başarıyla bağlanıldı');
        connection.release();
    })
    .catch(err => {
        console.error('❌ MySQL bağlantı hatası:', err.message);
    });

module.exports = pool;

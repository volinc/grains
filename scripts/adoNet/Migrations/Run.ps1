$coonection_string = "postgresql://postgres:pass@localhost:30008/grains"

function run($file) {
    psql -f $file -w ${coonection_string}
}

run("PostgreSQL-Clustering-3.6.0.sql")
run("PostgreSQL-Clustering-3.7.0.sql")
run("PostgreSQL-Persistence-3.6.0.sql")
run("PostgreSQL-Reminders-3.6.0.sql")
          
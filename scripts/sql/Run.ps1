$coonection_string = "postgresql://postgres:pass@localhost:30008/grains"

function run($file) {
    psql -f $file -w ${coonection_string}
}

run("PostgreSQL-Main.sql")
run("PostgreSQL-Clustering.sql")
run("PostgreSQL-Persistence.sql")
run("PostgreSQL-Reminders.sql")
          
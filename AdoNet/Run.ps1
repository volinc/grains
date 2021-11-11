$coonection_string = "postgresql://postgres:pass@localhost/grains"

function run($file) {
    psql -f $file -w ${coonection_string}
}

run("PostgreSQL-Main.sql")
run("PostgreSQL-Clustering.sql")
run("PostgreSQL-Persistence.sql")
run("PostgreSQL-Reminders.sql")
          
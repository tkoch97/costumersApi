# espera o Postgres ficar pronto
host="$1"
shift
cmd="$@"

until PGPASSWORD=$POSTGRES_PASSWORD psql -h "$host" -U "$POSTGRES_USER" -c '\q'; do
  echo "Esperando o Postgres em $host..."
  sleep 2
done

exec $cmd
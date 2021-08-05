@ECHO OFF

docker rm -f todo-app-net
docker rm -f couchbase_todoapp
docker network rm todo-app

docker network create todo-app
docker run -d --network todo-app --network-alias couchbase --name couchbase_todoapp -p 8091-8094:8091-8094 -p 11210:11210 couchbase
timeout /t 5
docker exec couchbase_todoapp curl -v -X POST http://127.0.0.1:8091/pools/default -d memoryQuota=512 -d indexMemoryQuota=512
docker exec couchbase_todoapp curl -v http://127.0.0.1:8091/node/controller/setupServices -d services=kv%2cn1ql%2Cindex
docker exec couchbase_todoapp curl -v http://127.0.0.1:8091/settings/web -d port=8091 -d username=admin -d password=123456
docker exec couchbase_todoapp curl -i -u admin:123456 -X POST http://127.0.0.1:8091/settings/indexes -d 'storageMode=memory_optimized'
docker exec couchbase_todoapp curl -v -u admin:123456 -X POST http://127.0.0.1:8091/pools/default/buckets -d name=Main -d bucketType=couchbase -d ramQuotaMB=128 -d authType=none
docker run -d -p 8080:80 --network todo-app --name todo-app-net todoappchallenge:latest

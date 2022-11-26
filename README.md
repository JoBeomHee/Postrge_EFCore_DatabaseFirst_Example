## **개요**
* 이번 포스팅에서는 .NET Core 앱을 하나 생성 후, PostgreSQL 와 EF Core 를 Database First 방식으로 연동하는 방법에 대해서 알려 드립니다.

<br/>

## **도구 & 프레임워크**
* Visual Studio 2022
* .NET 6
* PostgreSQL Docker & pgAdmin

<br/>

## **PostgreSQL 설치 및 pgAdmin 연동**
* 제일 먼저 PostgreSQL 설치 및 pgAdmin 연동을 해야 합니다.
* 저 같은 경우에는 PostgreSQL 데이터베이스는 Docker 를 이용하여 실행해 주었고, pgAdmin 은 공식 홈페이지에서 설치 후 사용하였습니다.
* PostgreSQL 데이터베이스 docker-compose 내용은 다음과 같습니다.

```yml
version: "3.4"
services:
  db:
    image: postgres:latest
    container_name: postgres
    restart: always
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: "beombeomjojo"
      POSTGRES_PASSWORD: "1234"
    volumes:
      - C:\postgresql\data:/var/lib/postgresql/data
```

* pgAdmin 설치 및 연동 방법은 다음 URL 을 참고하시면 됩니다.
* https://jobeomhee.github.io/posts/PostgreSQL-GUI-%EB%8F%84%EA%B5%AC/

![1](https://user-images.githubusercontent.com/22911504/204082403-96b13e07-c60f-4694-9b29-69752268bb79.png)

<br/>

## **테스트 진행할 테이블 생성**
* PostgreSQL 설치 및 pgAdmin 실행이 완료 되면, 테스트 진행할 테이블을 생성합니다.
* 예제로 tasks 라는 이름의 테이블을 생성합니다.
* 테이블 생성 SQL 구문은 아래 내용을 복사하여 적용하시면 됩니다.

![2](https://user-images.githubusercontent.com/22911504/204082404-845a7fc5-64a1-4b71-ae89-9d4c71d2b6b1.png)

```sql
CREATE TABLE tasks
(
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description VARCHAR(1000) NOT NULL,
    is_completed BOLLEAN,
    created_on TIMESTAMP NOT NULL
)
```

* 위 SQL 구문을 실행하면, 다음과 같이 tasks 이름의 테이블이 생성된 것을 확인할 수 있습니다.
  
![3](https://user-images.githubusercontent.com/22911504/204082405-d31321fd-ca67-4ddc-9d8f-3f8a2f9be666.png)

<br/>

## **ASP.NET Core 6 MVC Application 생성**
* 데이터베이스에서 할 작업은 모두 마쳤습니다.
* Visual Studio 2022 를 실행 후, `ASP.NET Core 6 MVC Application` 프로젝트 하나를 생성합니다.

![4](https://user-images.githubusercontent.com/22911504/204082406-daa46d4a-4d49-453c-8937-d278bba2af69.png)

<br/>

## **PostgreSQL EFCore NuGet Package 추가**
* EF Core를 사용하여 .NET 애플리케이션을 PostgreSQL과 연결하려면 PostgreSQL 데이터베이스 용 .NET Data Provider 가 필요합니다.
* 그러기 위해, NuGet 패키지 Npgsql.EntityFrameworkCore.PostgreSQL 을 설치합니다.

![5](https://user-images.githubusercontent.com/22911504/204082410-64b4b61b-a315-4a9c-8c27-3482cd86372e.png)

* 다음으로 기존 데이터베이스에 대한 EF 프록시 클래스를 생성하려면 Entity Framework Core Tools 가 필요합니다.
* EF Core용 패키지 관리자 콘솔 도구를 가져오려면 Microsoft.EntityFrameworkCore.Tools 패키지를 설치합니다.

![6](https://user-images.githubusercontent.com/22911504/204082411-b11e5279-0dd4-4723-adc5-5df13707d7a2.png)

* 지금까지 총 아래 그림과 같이 2개의 NuGet Package 를 설치하였습니다.

![7](https://user-images.githubusercontent.com/22911504/204082412-4c647cda-3edd-4ecb-9d09-4f3a38467e8f.png)

<br/>

## **기존 데이터베이스에 대한 EF 프록시 생성**
* 필요한 모든 NuGet 패키지를 설치했으므로 이제 기존 PostgreSQL 데이터베이스에 대한 EF 프록시 클래스를 생성할 시간입니다.
* Visual Studio로 이동하여 `도구 > NuGet 패키지 관리자 > 패키지 관리자 콘솔` 을 선택합니다.
* 그리고 다음 명령을 실행합니다.

```cmd
Scaffold-DbContext “Host=localhost;Database=ToDoManager;Username=DBUsername;Password=DBPassword” Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models
```

![8](https://user-images.githubusercontent.com/22911504/204082413-f0e5e29d-c17a-4771-b27a-74e2c880bc4e.png)

* 위 명령어가 성공적으로 실행 되었다면, `Model` 디렉터리가 다음과 같이 새롭게 생성된 것을 확인하실 수 있습니다.

![9](https://user-images.githubusercontent.com/22911504/204082414-d4a7b20f-d1c8-4931-a19f-47d8b024c58e.png)

<br/>

## **Entity Framework를 사용하도록 .NET 앱 구성**
* 이제 EF Core를 사용하도록 앱을 구성해 보겠습니다.
* appsettings.json 파일로 이동하여 아래와 같이 연결 문자열을 추가합니다.
* 아래에 `ConnectionStrings` 설정 키를 추가하고, 그 안에는 `MireroContext` 키와 DB 정보의 Value 값을 추가해 주었습니다.
 
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MireroContext": "Host=localhost;Database=mirero;Username=mirero;Password=system"
  }
}
```

* `Program.cs` 파일로 이동하여 종속성 주입을 위해 IoC Container 에 DBContext 를 등록해줍니다.

```csharp
builder.Services.AddDbContext<MireroContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("MireroContext")));
```

![10](https://user-images.githubusercontent.com/22911504/204082415-e8fb3a1d-78ea-4805-9f76-8c95ef923f06.png)

<br/>

## **실행 확인**
* 그럼 실제로 EF Core 와 Postgres 가 연동 되어 `Tasks` 테이블의 데이터를 정상적으로 가져오는지 확인합니다.
* 테스트를 위해, Postgres 에서 `Tasks` 테이블에 1개의 데이터를 미리 삽입해 놓았습니다.
* 그리고, C# 코드에서 데이터를 조회하여 출력 결과를 캡처하였습니다.
* 확인 결과, 정상적으로 DB 에 접근하여 데이터를 가져오는 것을 확인할 수 있습니다.

![11](https://user-images.githubusercontent.com/22911504/204082416-7025c873-4cf8-4e01-b442-5b532169f3b2.png)

![12](https://user-images.githubusercontent.com/22911504/204082417-bdde2259-75b5-490d-b535-00b92a5ee56e.png)

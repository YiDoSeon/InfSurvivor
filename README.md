# InfSurvivor
<video controls src="Resource/deom.mp4" title="Title" width="1280" height="720"></video>

## 클라이언트 Github

* 🛸 **Client**: [InfSurvivor Client](https://github.com/YiDoSeon/InfSurvivorClient)

## 1. 프로젝트 개요

- **목표**: 로그라이크 장르의 멀티플레이 게임 구현을 위한 고성능 서버 프레임워크 제작 및 유니티 엔진 연동
- **핵심 성과**
  - `MessagePack`을 활용한 고속 직렬화 시스템 구축
  - 순환 버퍼(`RecvBuffer`) 기반의 안정적인 패킷 수신 처리
  - 서버-클라이언트 간의 물리 동기화를 위한 자체 물리 엔진(Physics) 기초 설계

## 2. 시스템 아키텍처

### Core Network Layer (Common)

- **Session Management** : 비동기 I/O 기반의 소켓 관리 및 `SendBuffer` (Queue), `RecvBuffer` (순환 버퍼)를 통한 데이터 흐름 제어
- **Data Serialization**: `MessagePack`을 채택하여 JSON 대비 데이터 크기 최소화 및 직렬화 성능 극대화. C# 언어 사용으로 작업 효율 향상
- **Common Math**: 서버와 클라이언트의 연산 결과 일치를 위해 `CVector2`, `CVector3` 등 독자적인 수학 구조체 구현

### Server Logic (C# Console)

- **Main Game Loop**: 30 FPS 고정 프레임워크 기반의 `GameLogic`을 통해 연산 일관성 유지
- **Scalability**: `GameRoom` 단위의 공간 분리를 통해 다수의 동시 접속자가 독립적인 게임 환경을 가질 수 있도록 설계
- **Session Control**: `SessionManager`를 통한 클라이언트 생명주기 관리

### Client Logic (Unity)

- **FSM Based Controller**: 객체의 상태를 상태 패턴으로 관리하여 유연한 애니메이션 및 로직 확장 가능
- **Resource Management**: `AssetBundle` 기반의 리소스 캐싱 시스템을 구축하여 메모리 최적화 및 로딩 속도 개선

## 3. 문제 해결 사례

### 공간 분할 기반 충돌 감지

- 문제 : 모든 오브젝트 간 충돌 체크 시 O(n^2)의 비용이 발생하여 서버 부하 급증
- 해결 : `CollisionWorld`에 Grid Based 공간 분할 알고리즘을 적용하여 주변 객체들하고만 충돌을 검사하도록 최적화

### 순환 버퍼를 이용한 패킷 조림

- 문제 : TCP 특성상 패킷이 쪼개지거나 뭉쳐서 올 때 발생하는 데이터 오염 문제
- 해결 : `RecvBuffer` 내부에서 `Read/Write` 커서를 관리하는 순환 버퍼 구조를 직접 구현하여, 완전한 패킷이 구성될 때까지 데이터를 대기시키고 처리하는 매커니즘 구축

## 4. 핵심 요소

- `Physics`: `Circle`, `Box` 콜라이더를 지원하며 IColliderTrigger 인터페이스를 통해 컨텐츠 개발 편의성 제공
- `PacketHandler`: 자동 생성된 코드를 활용하여 수신된 패킷 ID에 맞는 핸들러를 즉시 호출하는 분기 시스템

## 5. 앞으로의 계획

### 1. 컨텐츠 동기화

- **전투 시스템 동기화**: RPC 및 패킷 설계를 통한 스킬 판정, 대미지 계산 등 핵심 전투 로직의 서버/클라이언트 동기화 R&D
- **물리 엔진 확장**: 현재의 충돌 감지 수준을 넘어, 이동 불가능 구역에 대한 충돌 반사 및 위치 보정 로직 구현
- **로비 및 매칭 시스템**: 다중 GameRoom 관리를 위한 로비 서버 구조 설계 및 유저 간 매칭 시스템 구축

### 2. 에셋 관리 시스템 R&D

- **Low-level 제어**: 고수준 API인 `Addressables` 대신 `AssetBundle`을 직접 다루는 것으로 리소스의 로드/언로드 시점과 메모리 생명주기를 정밀하게 제어
- **빌드 파이프라인 자동화**: 에셋 번들의 빌드, 압축, 배포 과정을 자동화하는 커스텀 빌드 시스템 구축을 통해 유지보수 효율성 증대

### 3. 멀티플랫폼 확장

- **입력 시스템**: 모바일 터치 인터페이스와 PC 키보드/마우스 입력을 동시에 지원하는 통합 입력 시스템 구현
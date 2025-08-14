
![BubblyShark](https://github.com/user-attachments/assets/c85dcfa5-2a52-48e1-af10-73e218433531)

# Project-Last | Final Project

Unity 2022.3 LTS 기반 URP 프로젝트

2.5D Top-down 시점에서 진행되는 협동 생존 호러 RPG입니다.

플레이어는 동료들과 함께 혹은 홀로 위험한 지역을 탐사하고, 자원을 수집해 탈출해야 합니다.  
간단한 조작과 협동 메커니즘을 통해 생존의 긴장감과 팀플레이의 재미를 동시에 제공합니다.

## Develop Environment

- Unity Version: 2022.3.17f1
- Render Pipeline: Universal Render Pipeline (URP)

## Game Features

### Controls
**Move**
- WASD: 기본 이동(Move)
- Shift: 달리기(Sprint/Run)
- Ctrl: 웅크리기(Sneak/Crouch)
- Spacebar: 대쉬(Dash)

**Action**
- Left Click: Normal Attack
- Right Click: Special Action
- E: E skill

**Interaction**
- B: 인벤토리(Inventory)
- F: 상호작용(Interaction)
- G: 모두 줍기(주변의 모든 템 줍기)(Root All)
- M: 미니맵(Minimap)

**etc**
- ESC: Option

상호작용의 경우 마우스를 올리고 있는 오브젝트와 상호작용됩니다.
(아이템 줍기, NPC 대화 등)

## 팀 구성

| 역할 | 이름 | 담당 업무 |
|------|------|------|
| Team Leader | 이창주 | 플레이어/몬스터 기반 시스템(FSM, 비헤이비어 트리 등), 세이브/로드 시스템, Lobby Scene
| Sub-Leader | 한수정 | 맵(랜덤 맵 생성, 프리팹 제작), NPC(다이얼로그 시스템)
| Developer | 장세희 | 아이템(소모품, 골동품, 맵에 랜덤 배치), 인벤토리(시스템 및 UI), 몬스터 추가 구현
| Developer | 윤창민 | 상점(NPC 및 기능), 퀘스트 데이터 관리, 사운드 시스템

## Git Commit Convention

| 타입         | 설명                     | 예시                         |
| ---------- | ---------------------- | -------------------------- |
| `feat`     | 새로운 기능 추가              | `feat: 플레이어 점프 기능 추가`      |
| `fix`      | 버그 수정                  | `fix: 충돌 시 HP 안 깎이던 문제 수정` |
| `chore`    | 설정, 빌드, 패키지 등 비기능적 작업  | `chore: Unity 프로젝트 초기화`    |
| `docs`     | 문서 수정 (README 등)       | `docs: 팀 구성 정보 업데이트`       |
| `style`    | 코드 스타일 변경 (공백, 들여쓰기 등) | `style: 코드 정리 및 네이밍 통일`    |
| `refactor` | 리팩토링 (기능 변화 없음)        | `refactor: 플레이어 이동 구조 개선`  |
| `test`     | 테스트 코드 추가/수정           | `test: 적 AI 유닛 테스트 추가`     |
| `perf`     | 성능 개선                  | `perf: 파티클 처리 최적화`         |

## Git Branch strategy

```css
main        ← 최종 제출/배포용
└─ dev      ← 개발 통합 브랜치
   ├─ feature/플레이어이동
   ├─ feature/UI구현
   ├─ fix/충돌버그
   └─ chore/셋업수정
```

| 타입          | 형식            | 설명                             |
| ----------- | ------------- | ------------------------------ |
| `main`      | `main`        | 최종 제출용 브랜치 (항상 안정 상태 유지)       |
| `dev`       | `dev`         | 통합 개발 브랜치. 각 기능 브랜치를 병합하는 중간지점 |
| `feature/*` | `feature/기능명` | 신규 기능 개발용 브랜치                  |
| `fix/*`     | `fix/버그명`     | 버그 수정 전용 브랜치                   |
| `chore/*`   | `chore/작업명`   | 설정/빌드/문서 등 비기능 브랜치             |
| `docs/*`    | `docs/문서명`    | README 등 문서 변경 브랜치             |

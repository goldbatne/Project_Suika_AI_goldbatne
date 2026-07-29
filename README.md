# Project_Suika_AI_goldbatne

NAN_CONTEST 출품작 — Unity로 만든 수박게임(Suika Game)에 AI를 접목한 프로젝트입니다.

## 소개

떨어지는 과일을 합쳐 더 큰 과일을 만드는 수박게임을 Unity로 구현하고,
AI가 게임을 플레이하도록 설계한 프로젝트입니다.

## 실행 방법

### 1. APK로 바로 플레이
Android 기기에 아래 파일을 설치해 실행합니다.
```
goldbatne_subak_game.apk   (프로젝트 루트에 위치)
```

### 2. Unity에서 열기
1. 저장소를 클론합니다.
   ```bash
   git clone https://github.com/goldbatne/Project_Suika_AI_goldbatne.git
   ```
2. Unity Hub에서 프로젝트를 엽니다.
3. **처음 열면 씬이 비어 있거나 오류가 표시될 수 있습니다.** 정상이며,
   아래 씬을 직접 열어 주세요. (Unity가 `Library`를 재생성하는 동안 로딩에 시간이 걸립니다)

#### 열어야 할 씬
| 씬 | 경로 | 설명 |
|----|------|------|
| 메인화면 | `Assets/Scenes/TitleScene` | 시작 화면. **여기부터 실행하세요.** |
| 게임화면 | `Assets/Scenes/GameScene` | 실제 게임 플레이 화면 |

> 게임을 처음부터 정상적으로 실행하려면 **`TitleScene`을 열고 Play**를 눌러 주세요.

## 사용 기술

- **엔진**: Unity (IL2CPP 빌드)
- **플랫폼**: Android
- **AI**: Claude, Gemini 활용
  - **개발 보조**: 코드 작성 및 디버깅
  - **게임 플레이 AI**: 게임 내 플레이 판단 로직

## 프로젝트 구조

- `Assets/` — 게임 소스, 스크립트, 에셋 (씬은 `Assets/Scenes/`)
- `Packages/` — 패키지 설정
- `ProjectSettings/` — 프로젝트 설정

## 라이선스

(필요 시 라이선스 기입)

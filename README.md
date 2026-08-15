# Project_Suika_AI_goldbatne

Unity로 만든 수박게임(Suika Game)에 AI를 접목한 프로젝트입니다.

## 소개

떨어지는 과일을 합쳐 더 큰 과일을 만드는 수박게임을 Unity로 구현하고,
AI를 활용해 기획·구현·검증 전 과정을 진행한 프로젝트입니다.

직접 코드를 작성하는 대신, 원하는 기획 의도를 명세로 정의하고 AI에게 구현을 지시한 뒤
플레이로 검증하고 다시 다듬는 방식으로 제작했습니다.

## 실행 방법

### 1. APK로 바로 플레이

Android 기기에 아래 파일을 설치해 실행합니다.

```
goldbatne_subak_game.apk   (프로젝트 루트에 위치)
```

- 다운로드: 위 파일 목록에서 `goldbatne_subak_game.apk` 클릭 → **Download**
- 설치: '출처를 알 수 없는 앱 설치 허용'이 필요할 수 있습니다.
- 조작: 화면을 터치·드래그해 과일을 조준하고, 손을 떼면 낙하합니다.

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

## 게임 특징

- **11단계 과일 합체** — 체리에서 시작해 최종 수박까지, 같은 과일을 합쳐 진화시킵니다.
- **흔들기 스킬** — 원작에 없는 오리지널 시스템. 위기 시 과일을 아래로 끌어당겨 판을 정리합니다.
- **경제 시스템** — 합체로 얻은 코인으로 흔들기 스킬을 구매하는 유한 자원 구조입니다.
- **화면비 대응** — 다양한 세로 화면 비율(16:9 ~ 20:9)에서 동일한 플레이 화면을 제공합니다.

## 사용 기술

- **엔진**: Unity (IL2CPP 빌드)
- **플랫폼**: Android
- **AI 활용**: 게임 제작 전 과정에 AI를 도구로 활용
  - **Gemini**: 기획 상담 및 C# 코드 구현 가이드 (대화로 상담 후 직접 적용)
  - **Claude**: 프로젝트 분석·검증 및 밸런싱 개편 (프롬프트로 파일 직접 수정)

## 프로젝트 구조

- `Assets/` — 게임 소스, 스크립트, 에셋 (씬은 `Assets/Scenes/`)
- `Packages/` — 패키지 설정
- `ProjectSettings/` — 프로젝트 설정
- `goldbatne_subak_game.apk` — 빌드된 Android 실행 파일

## 알려진 이슈 — SpriteAtlas 관련 에러 로그

git clone이 아닌 **ZIP 다운로드 후 압축 해제**로 프로젝트를 열면,
폴더가 중첩되어 경로가 길어지고 Windows 경로 길이 제한(260자)에
걸려 Unity 에디터 내부 SpriteAtlas 검사 UI에서 에러 로그가 발생할 수 있습니다.
(`DirectoryNotFoundException`, 경로에 `-main` 폴더가 중복 표시됨)

**게임 실행·빌드에는 영향이 없으며**, 실제 원인은 프로젝트 경로가 아니라
Unity 에디터의 SpriteAtlas 리포트 UI 리소스 로딩 문제입니다.

권장: 아래처럼 clone으로 받아 폴더 중첩을 피해 주세요.
\`\`\`bash
git clone https://github.com/goldbatne/Project_Suika_AI_goldbatne.git
\`\`\`
또는 ZIP을 받았다면 압축 해제 후 C 드라이브 루트에 가까운 짧은 경로
(예: `C:\Suika\`)로 옮겨서 열어 주세요.

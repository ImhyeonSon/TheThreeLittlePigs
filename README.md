# 🐷악의 돼지 삼형제🐺

<div align="center">
23.10.09 ~ 23.11.17 (6주)<br/>
</div>

## 서비스 소개
<div align="center">
<b>연령에 관계없이 모두가 즐길 수 있는 비대칭 PVP 서바이벌 게임! 🐷와 🐺 중 한 팀을 선택하고, 게임에서 승리하세요!🙋</b><br/><br/>

![](images/gifs/Main.gif)
</div>

## 개요
동화속 이야기 '아기 돼지 삼형제'의 돼지와 늑대가 되어서 동화 속 이야기를 완성해 가세요!

## Why?
기존의 비대칭 PvP 게임은 공포 / 스릴러 장르가 다수였습니다. 저희는 어두운 분위기가 아닌 밝은 분위기의 누구나 즐길 수 있는 비대칭 PvP 게임을 만들고 싶었습니다. 많은 사람들에게 친숙한 '아기 돼지 삼형제' 동화 속 스토리에 기반해 게임을 만들었습니다. 돼지는 벽돌집을 지어 가보를 안전하게 보관하고, 늑대는 그 전에 돼지와 집을 공격해 가보를 빼앗으세요! 

## 서비스 특징
1. 밝은 분위기로 누구나 쉽게 즐길 수 있는 게임
2. 동화에 기반해 누구나 쉽게 이해할 수 있는 게임 배경 스토리
3. 돼지는 아이템 제작 및 수비, 늑대는 사냥을 통한 레벨업 및 공격. <br>
    확연히 다른 두 캐릭터의 컨셉으로 캐릭터마다 다른 재미를 느낄 수 있음

## 승리 조건
### 🐷 돼지 🐷
집을 업그레이드 시켜 벽돌 집으로 만들어 그 안에 "가보"를 넣으면 승리

### 🐺 늑대 🐺
돼지가 벽돌 집에 가보를 넣기 전에 돼지와 집을 공격해 "가보"를 빼앗으면 승리

## 기능별 화면 

#### [> 공통 기능 GIF 보러가기 <](./exec/theThrillLittlePig-UI.md)
#### [> 🐷 플레이어 GIF 보러가기 <](./exec/theThrillLittlePig-Pig.md)
#### [> 🐺 플레이어 GIF 보러가기 <](./exec/theThrillLittlePig-Wolf.md)

---

## 개발 환경
### Game Engine
- Unity Engine
- C#

## 협업 툴
- Git
- Jira
- Notion
- Mattermost
- Plastic SCM
- Google Drive

## 요구사항 정의서
- [아기 돼지 삼형제 요구사항 정의서](https://www.notion.so/107de4fc5f4d48cc9208d1404a2c91b3?v=e71d63270eb84d6aa60255da33b9576f&p=e14ae19c81d347928c5681a785d38c0b&pm=s)
- [게임 상세 내용](https://www.notion.so/05cf60b5feea4c2096b8a340651e5f70?pvs=4)


## 문서 링크
- [CCK 팀의 노션](https://www.notion.so/a9f2781301024a5dafa7017a6c3cee2c?v=887dffc746d649b3b37c545d16e3e92c)
- [요구사항 정의서](https://unique-work-649.notion.site/05cf60b5feea4c2096b8a340651e5f70?pvs=4)

## Git-flow

- Merge 할 때, dev브랜치에서 자신의 브랜치로 먼저 Merge한 뒤, 에러 유무를 확인 후 dev branch로 Merge한다.

- Branch Type

  > master : 운영 서버로 배포하기 위한 branch<br/>
  > develop : 다음 출시 기능을 개발하는 branch<br/>
  > feature : 세부 기능을 개발하는 branch, branch 이름은 각 기능명_팀원명으로 작성<br/>
  > hotfix : 급한 에러 수정

- feature branch 이름 규칙
  > feature_[기능명]_[팀원명]<br/>
  > ex) feature_Character_Jay<br/>


## Plastic SCM conventions

### Type 분류
> FEAT: 새로운 기능 및 파일을 추가할 경우<br/>
> MODIFY: 기능을 변경한 경우<br/>
> STYLE: 간단한 수정, 코드 변경이 없는 경우<br/>
> FIX: 버그를 고친 경우<br/>
> DOCS: 문서를 수정한 경우(ex> Swagger, README)<br/>
> COMMENT: 주석 추가/삭제/변경한 경우<br/>
> RENAME: 파일 혹은 폴더명 수정 및 이동<br/>
> DELETE: 파일 혹은 기능 삭제<br/>
> CHORE: 빌드 업무 수정(ex> dependency 추가)<br/>
> REFATOR: 프로덕션 코드 리팩토링<br/>
> MERGE: 충돌 시 머지, develop 브랜치에 풀리퀘 등


### 형식
[Type] 내용 (#[Jira 이슈번호]) ex. [FEAT] 로그인 기능 구현 #S09P22C210-174


## 팀원 소개 및 역할

<strong>전종헌</strong>
- 팀장
- 늑대 캐릭터 기능 구현
- 늑대 캐릭터 스킬 구현
- 늑대 캐릭터 애니메이션 구현
- 집 건설, 업그레이드, 파괴 애니메이션 구현
- 발표

<strong>손임현</strong>
- 캐릭터 UI/UX
- 대기 방 UI
- 포톤 서버 구현
  > 로비 구현<br/>
  > 방 구현 (방 생성, 비밀번호)
- 게임 로직 구현
- 게임 시작 플레이어 동기화
- 스크립트 구조 설계
- 플레이어 기본 조작기능 구현
- 늑대 캐릭터 스킬 구현
- 돼지, 늑대 애니메이션 구현
- 채팅 기능 구현
- 야생동물 구현
- 미니맵 구현
- 가보 구현
- RPC 동기화

<strong>양수원</strong>
- 집 건설 및 업그레이드, 스탯, 내구도 관리
- 게임 내부 인벤토리 로직 관리 및 설계
- 집 내부 위치한 상자 인벤토리 이용 기능 구현
- 집 내부 위치한 제작대에서 아이템 제작 기능 구현
- 맵 설계
- 맵 디자인

<strong>오정원</strong>
- 돼지 캐릭터 기능 구현
- 돼지 캐릭터 애니메이션 구현
- 돼지 캐릭터 아이템 이용 설계 및 파밍 기능 구현
- 돼지 캐릭터 집과 상호작용 기능 구현
- 가보 UI 구현
- UCC 제작

<strong>윤예지</strong>
- 시스템 UI/UX
- 맵 설계
- 파밍 오브젝트 생성 및 리스폰 구현
- 집터 및 집 짓기 동기화
- 체력바, 진행바 모듈화
- 튜토리얼 제작
- 메인화면, 게임 결과 화면 제작
- PPT 제작

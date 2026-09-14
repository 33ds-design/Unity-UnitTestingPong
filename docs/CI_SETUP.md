# CI 配置指南

## 前置条件

### 1. Fork 项目到你的 GitHub

```bash
# 在 GitHub 上 fork 原项目，然后：
git remote set-url origin https://github.com/<你的用户名>/unity-pong-test.git
git push -u origin main
```

### 2. 获取 Unity License 文件

game-ci/unity-test-runner 需要 Unity 许可证才能在 CI 中运行。

#### 获取 Personal License（免费）：

1. 在本地安装标准 Unity 2022.3 LTS（非团结引擎）
2. 打开 Unity Hub → Settings → License Management
3. 选择 "Obtain a Personal Edition license"
4. 保存 `.ulf` 文件

#### 配置 GitHub Secrets

在 GitHub 仓库 → Settings → Secrets and variables → Actions → New repository secret：

| Secret 名称 | 值 | 说明 |
|---|---|---|
| `UNITY_LICENSE` | `.ulf` 文件的完整内容（Base64 编码） | Unity 许可证 |
| `CODECOV_TOKEN` | 从 codecov.io 获取的 token | 覆盖率上传 |

#### Base64 编码 License：

```bash
# Linux/Mac
base64 -i Unity_v2022.ulf > unity_license_base64.txt

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("Unity_v2022.ulf")) > unity_license_base64.txt
```

将 `unity_license_base64.txt` 的内容粘贴到 `UNITY_LICENSE` secret 中。

### 3. 启用 Codecov

1. 访问 https://codecov.io 并使用 GitHub 账号登录
2. 添加你的仓库
3. 获取 `CODECOV_TOKEN` 并添加到 GitHub Secrets

## CI 工作流说明

### 工作流文件

`.github/workflows/unity-test.yml` 包含 3 个 job：

| Job | 说明 | 运行条件 |
|---|---|---|
| `editmode-tests` | 运行 EditMode 单元测试 | push / PR |
| `playmode-tests` | 运行 PlayMode 集成测试 | push / PR |
| `coverage` | 运行测试并生成覆盖率报告 | push / PR |

### 团结引擎兼容性处理

CI 使用 `scripts/patch-for-ci.sh` 脚本自动处理团结引擎与标准 Unity 的差异：

1. 将 `ProjectVersion.txt` 中的 `2022.3.61t14` 改为 `2022.3.61f1`
2. 从 `manifest.json` 中移除团结引擎专有包：
   - `cn.tuanjie.codely.bridge`
   - `cn.tuanjie.ai.generators`
   - `com.coplaydev.unity-mcp`（MCP 插件，CI 中不需要）

### 本地运行覆盖率

```powershell
# Windows (团结引擎)
.\scripts\run-coverage.ps1 -Mode EditMode

# 运行所有测试（EditMode + PlayMode）
.\scripts\run-coverage.ps1 -Mode All
```

结果输出到 `test-results/` 和 `coverage-report/` 目录。

## CI 徽章

在 README.md 中添加：

```markdown
![Unity Tests](https://github.com/<用户名>/unity-pong-test/actions/workflows/unity-test.yml/badge.svg)
[![codecov](https://codecov.io/gh/<用户名>/unity-pong-test/branch/main/graph/badge.svg)](https://codecov.io/gh/<用户名>/unity-pong-test)
```

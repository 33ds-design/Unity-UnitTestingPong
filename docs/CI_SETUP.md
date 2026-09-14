# CI 配置指南

## 前置条件

### 1. Fork 项目到你的 GitHub

```bash
# 在 GitHub 上 fork 本项目，然后：
git clone https://github.com/<你的用户名>/Unity-UnitTestingPong.git
cd Unity-UnitTestingPong
```

### 2. 配置 Unity 许可证

CI 使用 **serial 激活方式**（账号 + 密码 + 序列号），需要三个 GitHub Secrets。

#### 获取 Unity Personal License 序列号

1. 安装标准 [Unity Hub](https://unity.com/download)
2. 登录 Unity 账号
3. `Unity Hub` → `Preferences` → `Licenses` → `Add`
4. 选择 **Get a free personal license**
5. 许可证激活后，序列号格式为 `XX-XXXX-XXXX-XXXX-XXXX-XXXX`

#### 配置 GitHub Secrets

在 `GitHub 仓库` → `Settings` → `Secrets and variables` → `Actions` → `New repository secret`：

| Secret 名称 | 值 | 说明 |
|---|---|---|
| `UNITY_SERIAL` | Unity 序列号（如 `F4-K7CC-3G9C-Q6NV-G3CK-DGX4`） | Unity 许可证序列号 |
| `UNITY_EMAIL` | Unity 账号邮箱 | 登录 Unity Hub 用的邮箱 |
| `UNITY_PASSWORD` | Unity 账号密码 | 登录 Unity Hub 用的密码 |
| `CODECOV_TOKEN` | 从 codecov.io 获取的 token | 覆盖率上传用 |

> **注意**：`UNITY_LICENSE`（ULF 文件方式）也可用，但 Tuanjie Engine 生成的 ULF 文件与标准 Unity Docker 镜像的数字签名不兼容，因此推荐使用 serial 方式。

#### 已知问题：ULF 文件签名不兼容

如果使用 Tuanjie Engine 生成的 `Tuanjie_lic.ulf` 作为 `UNITY_LICENSE` Secret，CI 会报错：

```
Cannot load ULF license: The digital signature is invalid.
```

原因：Tuanjie Engine 的许可证签名不同于标准 Unity，Docker 镜像中的 Unity 编辑器无法验证通过。解决方式是使用 serial 激活。

### 3. 启用 Codecov

1. 访问 [codecov.io](https://codecov.io) 并使用 GitHub 账号登录
2. 添加你的仓库（`33ds-design/Unity-UnitTestingPong`）
3. 获取 `CODECOV_TOKEN` 并添加到 GitHub Secrets

## CI 工作流说明

### 工作流文件

`.github/workflows/unity-test.yml` 包含 3 个并行 job：

| Job | 说明 | 超时 | 产出 |
|---|---|---|---|
| `editmode-tests` | 运行 EditMode 单元测试（87 个） | 30 min | XML 测试结果 |
| `playmode-tests` | 运行 PlayMode 集成测试（34 个） | 30 min | XML 测试结果 |
| `coverage` | 运行测试并生成覆盖率报告 | 30 min | Cobertura XML → Codecov |

### 许可证激活流程

每个 job 的 `game-ci/unity-test-runner@v4` step 通过 `env` 传递许可证变量：

```yaml
- name: Run Unity Tests
  uses: game-ci/unity-test-runner@v4
  env:
    UNITY_SERIAL: ${{ secrets.UNITY_SERIAL }}
    UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
    UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
  with:
    unityVersion: '2022.3.48f1'
    projectPath: .
    testMode: EditMode  # or PlayMode
```

### 团结引擎兼容性处理

CI 使用 `scripts/patch-for-ci.sh` 脚本自动处理 Tuanjie Engine 与标准 Unity 的差异：

1. 将 `ProjectVersion.txt` 中的 `2022.3.61t14` 改为 `2022.3.48f1`
2. 移除 `manifest.json` 中 Tuanjie 专有包（`cn.tuanjie.*`）
3. 移除 MCP 插件包（`com.coplaydev.unity-mcp`，CI 中不需要）
4. 添加 `com.unity.testtools.codecoverage` 包（覆盖率报告依赖）
5. 删除 `packages-lock.json` 让 Unity 自动重新解析依赖

### 覆盖率报告流程

```
Unity Test Runner (OpenCover XML)
        ↓
ReportGenerator (Cobertura XML)
        ↓
Codecov Action (上传)
```

Assembly 过滤器：`+Assembly-CSharp,+Pong`（仅统计游戏代码，不含测试代码）

### 本地运行覆盖率

```powershell
# Windows (Tuanjie/Unity Editor)
.\scripts\run-coverage.ps1 -Mode EditMode

# 运行所有测试（EditMode + PlayMode）
.\scripts\run-coverage.ps1 -Mode All
```

结果输出到 `test-results/` 和 `coverage-report/` 目录。

## CI 徽章

README.md 中已包含：

```markdown
![Unity Tests](https://github.com/33ds-design/Unity-UnitTestingPong/actions/workflows/unity-test.yml/badge.svg)
[![codecov](https://codecov.io/gh/33ds-design/Unity-UnitTestingPong/branch/master/graph/badge.svg)](https://codecov.io/gh/33ds-design/Unity-UnitTestingPong)
```

## 故障排查

### 许可证相关

| 错误信息 | 原因 | 解决方案 |
|----------|------|----------|
| `Cannot load ULF license: Data at the root level is invalid` | UNITY_LICENSE 存储的是 Base64 编码而非原始 XML | 粘贴原始 XML 内容，或改用 serial 激活 |
| `Cannot load ULF license: The digital signature is invalid` | Tuanjie ULF 签名与标准 Unity 不兼容 | 改用 serial 激活方式 |
| `Failed to return the Unity license after 4 attempts` | 测试完成后归还许可证失败 | 非致命警告，不影响测试结果 |

### 测试相关

| 错误信息 | 原因 | 解决方案 |
|----------|------|----------|
| `SendMessage("OnTriggerEnter2D") triggers ShouldRunBehaviour()` | EditMode 下 MonoBehaviour 生命周期检查失败 | 使用反射直接调用 `OnTriggerEnter2D` 方法 |
| AI 挡板不移动 | `Time.deltaTime` 在 CI 中为 0 | 使用反射设置 `_aiTimer` 字段绕过延迟 |
| `LessThanOrEqualTo` 不接受 `.Within()` | NUnit 约束 API 限制 | 移除 `.Within()` 或改用 `Is.EqualTo(x).Within(tolerance)` |

#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUTPUT_DIR="${1:-$ROOT_DIR/artifacts/coverage}"
RESULTS_DIR="$OUTPUT_DIR/test-results"
REPORT_DIR="$OUTPUT_DIR/report"

export PATH="$PATH:$HOME/.dotnet/tools"
export DOTNET_ROLL_FORWARD="${DOTNET_ROLL_FORWARD:-Major}"

declare -a TEST_PROJECTS=(
  "$ROOT_DIR/Domains/Weather/Tests/Behaviour/Behaviour.csproj"
  "$ROOT_DIR/Library/Aviant/tests/EventSourcing/Unit/Unit.csproj"
)

rm -rf "$OUTPUT_DIR"
mkdir -p "$RESULTS_DIR" "$REPORT_DIR"

for test_project in "${TEST_PROJECTS[@]}"; do
  project_name="$(basename "$(dirname "$test_project")")"
  dotnet test "$test_project" \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --results-directory "$RESULTS_DIR/$project_name"
done

if command -v reportgenerator >/dev/null 2>&1; then
  reportgenerator \
    -reports:"$RESULTS_DIR/**/coverage.cobertura.xml" \
    -targetdir:"$REPORT_DIR" \
    -reporttypes:"HtmlInline_AzurePipelines;Cobertura;TextSummary"
else
  mkdir -p "$REPORT_DIR"
  find "$RESULTS_DIR" -name 'coverage.cobertura.xml' -exec cp {} "$REPORT_DIR" \;
  {
    echo "Coverage artifacts collected."
    echo "HTML report generation was skipped because ReportGenerator could not run in the current SDK/runtime environment."
    echo "Cobertura files:"
    find "$RESULTS_DIR" -name 'coverage.cobertura.xml' | sort
  } > "$REPORT_DIR/Summary.txt"
fi

cat "$REPORT_DIR/Summary.txt"

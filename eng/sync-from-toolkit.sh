#!/bin/sh
# Usage: eng/sync-from-toolkit.sh <clone of CommunityToolkit/Aspire> <ref>
# Copies the Chroma integration of the toolkit at <ref> (the branch of the pull request, or main once it is merged)
# into this repository and commits it, with a Co-authored-by line for each author of the toolkit commits since the
# last sync. The last synced toolkit commit is kept in eng/toolkit-commit.txt.
set -e
toolkit="$1"; ref="$2"
[ -n "$toolkit" ] && [ -n "$ref" ] || { echo "Usage: $0 <clone of CommunityToolkit/Aspire> <ref>" >&2; exit 2; }
repo=$(git rev-parse --show-toplevel)
paths="src/CommunityToolkit.Aspire.Chroma src/CommunityToolkit.Aspire.Hosting.Chroma tests/CommunityToolkit.Aspire.Chroma.Tests tests/CommunityToolkit.Aspire.Hosting.Chroma.Tests examples/chromadb"

new=$(git -C "$toolkit" rev-parse "$ref")
old=$(cat "$repo/eng/toolkit-commit.txt" 2>/dev/null || true)
[ "$new" = "$old" ] && { echo "Already at $new."; exit 0; }

# The folders of the toolkit replace ours, deleted files included.
for p in $paths; do
  git -C "$repo" rm -rq --ignore-unmatch "$p"
  git -C "$toolkit" archive "$new" "$p" | tar -x -C "$repo"
done
echo "$new" > "$repo/eng/toolkit-commit.txt"
git -C "$repo" add -A $paths eng/toolkit-commit.txt

if git -C "$repo" diff --cached --quiet -- $paths; then
  echo "No change in the Chroma folders."
  git -C "$repo" commit -q -m "Record CommunityToolkit/Aspire $(echo "$new" | cut -c1-8) as synced"
  exit 0
fi

# Authors of the toolkit commits since the last sync that touch the Chroma folders, as GitHub noreply addresses
# when the commit carries a login, never other emails.
coauthors=""
if [ -n "$old" ] && git -C "$toolkit" merge-base --is-ancestor "$old" "$new" 2>/dev/null; then
  range="$old..$new"
else
  range="$new -1"
fi
me=$(gh api user --jq .login)
for sha in $(git -C "$toolkit" rev-list $range -- $paths); do
  login=$(gh api "repos/CommunityToolkit/Aspire/commits/$sha" --jq '.author.login // empty' 2>/dev/null || true)
  [ -z "$login" ] || [ "$login" = "$me" ] && continue
  id=$(gh api "users/$login" --jq .id)
  name=$(git -C "$toolkit" log -1 --format=%an "$sha" | cut -d' ' -f1)
  line="Co-authored-by: $name <$id+$login@users.noreply.github.com>"
  case "$coauthors" in *"$line"*) ;; *) coauthors="$coauthors
$line";; esac
done

git -C "$repo" commit -q -F - <<EOF
Sync with CommunityToolkit/Aspire $(echo "$new" | cut -c1-8)
$coauthors
EOF
git -C "$repo" log -1 --stat | cat

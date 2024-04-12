REPO_NAME=$(basename "$(git rev-parse --show-toplevel)")
echo "Repo: $REPO_NAME"
LAST_VERSION_TAG=$(git describe --tags --match="v[0-9]*\.[0-9]*" --abbrev=0)
echo "Last version tag: $LAST_VERSION_TAG"
MAJOR_MINOR_VERSION=$(echo $LAST_VERSION_TAG | grep -oP "[0-9]+\.[0-9]+")
echo "Major.Minor version: $MAJOR_MINOR_VERSION"

BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)
BRANCH_NAME_CLEANED=${BRANCH_NAME//[\/]/\.}
if [ $BRANCH_NAME = 'main' ]; then
REVISION=$(git rev-list --count "$LAST_VERSION_TAG..HEAD")
BRANCH_SUFFIX=""
else
REVISION=$(git rev-list --count "$LAST_VERSION_TAG..HEAD")
BRANCH_REVISION=$(git rev-list --count "origin/main..HEAD")
REVISION=$(($REVISION-$BRANCH_REVISION))
BRANCH_SUFFIX="-$BRANCH_NAME_CLEANED.$BRANCH_REVISION"
fi
echo "Revision: $REVISION"
echo "Branch: $BRANCH_NAME"
echo "Branch suffix: $BRANCH_SUFFIX"
BUILD_VERSION="$MAJOR_MINOR_VERSION.$REVISION$BRANCH_SUFFIX"
echo "Build version: $BUILD_VERSION"
# BUILD_NAME="${REPO_NAME}_${BUILD_VERSION}_${{matrix.targetPlatform}}"
BUILD_NAME="${REPO_NAME}_${BUILD_VERSION}"
echo "Build name: $BUILD_NAME"
EXECUTABLE_NAME=$REPO_NAME
echo "Executable name: $EXECUTABLE_NAME"
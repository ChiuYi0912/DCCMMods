const fs = require("fs");

const file = process.argv[2];
let lines = fs.readFileSync(file, "utf8").split("\n");

let stack = [];

for (let i = 0; i < lines.length; i++) {
  const trimmed = lines[i].trim();

  if (trimmed.startsWith("#if")) {
    let nextNonEmpty = i + 1;
    while (nextNonEmpty < lines.length && lines[nextNonEmpty].trim() === "") {
      nextNonEmpty++;
    }
    let indent = "";
    if (nextNonEmpty < lines.length) {
      const match = lines[nextNonEmpty].match(/^\s*/);
      indent = match ? match[0] : "";
    }
    stack.push(indent);
    lines[i] = indent + trimmed;
  }
  else if (trimmed.startsWith("#else") || trimmed.startsWith("#elif")) {
    if (stack.length > 0) {
      const indent = stack[stack.length - 1];
      lines[i] = indent + trimmed;
    }
  }
  else if (trimmed.startsWith("#endif")) {
    if (stack.length > 0) {
      const indent = stack.pop();
      lines[i] = indent + trimmed;
    }
  }
}

fs.writeFileSync(file, lines.join("\n"));
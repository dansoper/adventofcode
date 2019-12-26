const intCode = function(x, startP = 0, me = 0) {
  const xs = x.split(",").map(a => parseInt(a));
  let size = 4;
  for (p = startP; ; p += size) {
    const instruction = String(xs[p]);
    const op = parseInt(instruction.substring(instruction.length - 2));
    const parameterModes = [0,0,0,0,0,0,0,0,0,0];
    for (ii = 0; ii < instruction.length - 2; ii++) { 
      parameterModes[ii] = parseInt(instruction.substring(instruction.length - 2 - ii - 1, instruction.length - 2 - ii));
    }
    if (op != null) {

      if (op == 1) {
        if (parameterModes[2] == 1) throw "write parameterD";
        xs[xs[p+3]] = getParam(xs, parameterModes, p, 1) + getParam(xs, parameterModes, p, 2);
        size = 4;

      } else if (op == 2) {
        if (parameterModes[2] == 1) throw "write parameterC";
        xs[xs[p+3]] = getParam(xs, parameterModes, p, 1) * getParam(xs, parameterModes, p, 2);
        size = 4;

      } else if (op == 3) {
        let userInput = 5;//prompt("input");
		  if (position != null) {
		    userInput = position;
			 position = null;
		  } else if (lastOutput != null && me != lastOutputMe) {
			 userInput = lastOutput;
          lastOutput = null;
		  } else {
           console.log("Sleeping for input");
           return { memory: xs.toString(), p };
        }
                 console.log("using input " + userInput);
        if (parameterModes[0] == 1) throw "write parameter O";
        xs[xs[p+1]] = userInput;
        size = 2;

      } else if (op == 4) {
        lastOutput = getParam(xs, parameterModes, p, 1);
        lastOutputMe = me;
        console.log("output: " + getParam(xs, parameterModes, p, 1));
        size = 2;

      } else if (op == 5) {
        if (getParam(xs, parameterModes, p, 1) != 0) {
          p = getParam(xs, parameterModes, p, 2);
          size = 0;
        } else {
          size = 3;
        }

      } else if (op == 6) {
        if (getParam(xs, parameterModes, p, 1) == 0) {
          p = getParam(xs, parameterModes, p, 2);
          size = 0;
        } else {
          size = 3;
        }

      } else if (op == 7) {
        if (parameterModes[2] == 1) throw "write parameterB";
        size = 4;
        if (getParam(xs, parameterModes, p, 1) < getParam(xs,   parameterModes, p, 2)) {
          xs[xs[p+3]] = 1;
        } else {
          xs[xs[p+3]] = 0;
        }

      } else if (op == 8) {
        if (parameterModes[2] == 1) throw "write parameterA";
        size = 4;
        if (getParam(xs, parameterModes, p, 1) == getParam(xs,   parameterModes, p, 2)) {
          xs[xs[p+3]] = 1;
        } else {
          xs[xs[p+3]] = 0;
        }

      } else if (op == 99) {
        break;

      } else {
        throw "not expected!"+xs.toString();
      }
    } else {
      break;
    }
  }
  return { memory: xs.toString(), p: null };
}

const getParam = function(array, parameterModes, globalPosition, localPosition) { 
  if (parameterModes[localPosition - 1] == 1) {
    return array[globalPosition + localPosition];
  } else if (parameterModes[localPosition - 1] == 0) {
        return array[array[globalPosition + localPosition]];
  } else {
    throw "Not expected param mode!";
  }

}






let bestPhase = [];
let bestOutput = 0;

let lastOutput = null;
let lastOutputMe = null;
let position = 0;
let phaseSettings = [];
let originalInput = "3,8,1001,8,10,8,105,1,0,0,21,34,59,68,85,102,183,264,345,426,99999,3,9,101,3,9,9,102,3,9,9,4,9,99,3,9,1002,9,4,9,1001,9,2,9,1002,9,2,9,101,5,9,9,102,5,9,9,4,9,99,3,9,1001,9,4,9,4,9,99,3,9,101,3,9,9,1002,9,2,9,1001,9,5,9,4,9,99,3,9,1002,9,3,9,1001,9,5,9,102,3,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99";


//3,8,1001,8,10,8,105,1,0,0,21,34,59,68,85,102,183,264,345,426,99999,3,9,101,3,9,9,102,3,9,9,4,9,99,3,9,1002,9,4,9,1001,9,2,9,1002,9,2,9,101,5,9,9,102,5,9,9,4,9,99,3,9,1001,9,4,9,4,9,99,3,9,101,3,9,9,1002,9,2,9,1001,9,5,9,4,9,99,3,9,1002,9,3,9,1001,9,5,9,102,3,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99";

/*
for (i = 0; i <= 4; i++) {
  for (j = 1; j <= 4; j++) {
    if (i == j) continue;
    for (k = 0; k <= 4; k++) {
      if (i == k || j == k) continue;
      for (l = 0; l <= 4; l++) {
        if (i == l || j == l || k == l) continue;
        for (m = 0; m <= 4; m++) {
          if (i == m || j == m || k == m || l == m) continue;
          phaseSettings = [i,j,k,l,m];
          console.log(phaseSettings);
          lastOutput = 0;
         let input = originalInput
          phaseSettings.forEach((item) => {
				position = item; console.log(input);
                               intCode(input);
                               
          });
			 console.log(lastOutput);
                        if (lastOutput > bestOutput) {
                         bestOutput = lastOutput; bestPhase = phaseSettings;
                        }
        }
      }
    }
  }
}
*/

for (i = 5; i <= 9; i++) {
  for (j = 5; j <= 9; j++) {
    if (i == j) continue;
    for (k = 5; k <= 9; k++) {
      if (i == k || j == k) continue;
      for (l = 5; l <= 9; l++) {
        if (i == l || j == l || k == l) continue;
        for (m = 5; m <= 9; m++) {
          if (i == m || j == m || k == m || l == m) continue;
          phaseSettings = [i,j,k,l,m];
          let times = 0;
         let input = originalInput;
          let memories = [0,0,0,0,0, input, input, input, input, input];
          let ps = [0,0,0,0,0,0,0,0,0,0];
          console.log(phaseSettings);
          lastOutput = 0;
           lastOutputMe = 200;
          while (true) {
           times++;
          phaseSettings.forEach((item) => {
				if (times == 1) position = item; else position = null;
          console.log(memories[item]);
                                   
          const ret = intCode(memories[item], ps[item], item);


           ps[item] = ret.p;
           memories[item] = ret.memory;
                               
          });
           if(ps[5] == null && ps[6] == null && ps[7] == null && ps[8] == null && ps[9] == null) break;
			 }
			 console.log(lastOutput);
                        if (lastOutput > bestOutput) {
                         bestOutput = lastOutput; bestPhase = phaseSettings;
                        }
        }
      }
    }
  }
}

console.log("BO");

console.log(bestOutput);
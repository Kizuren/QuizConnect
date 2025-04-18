import React, { useState } from 'react';
import clsx from 'clsx';

interface CodeInputProps {
  length?: number;
  onComplete: (code: string) => void;
}

export const CodeInput: React.FC<CodeInputProps> = ({ length = 4, onComplete }) => {
  const [code, setCode] = useState<string[]>(Array(length).fill(''));
  const inputRefs = React.useRef<(HTMLInputElement | null)[]>([]);

  const handleChange = (index: number, value: string) => {
    if (value.length > 1) return;
    
    const newCode = [...code];
    newCode[index] = value;
    setCode(newCode);

    if (value && index < length - 1) {
      inputRefs.current[index + 1]?.focus();
    }

    if (newCode.every(digit => digit !== '')) {
      onComplete(newCode.join(''));
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Backspace' && !code[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  return (
    <div className="flex gap-4">
      {Array.from({ length }).map((_, index) => (
        <input
          key={index}
          ref={el => inputRefs.current[index] = el}
          type="text"
          maxLength={1}
          className={clsx(
            "w-14 h-14 text-center text-2xl rounded-lg",
            "bg-gray-800 border-2 border-gray-700",
            "focus:border-blue-500 focus:outline-none",
            "text-white placeholder-gray-500"
          )}
          value={code[index]}
          onChange={e => handleChange(index, e.target.value)}
          onKeyDown={e => handleKeyDown(index, e)}
        />
      ))}
    </div>
  );
};